namespace BlackJack.Models
{
    public class PlayerModel
    {
        const int CardsInOneDeck = 52;
        const int CardsInASuit = 13;

        public string Name { get; set; } = string.Empty;

        public List<int> Cards { get; set; } = new();

        public bool IsHidingFirstCard { get; set; }

        public int Score()
        {
            var total = 0;
            var aceCount = 0;
            var isFirstCard = true;

            foreach (var card in Cards)
            {
                if (IsHidingFirstCard && isFirstCard) // Don't count hidden card
                {
                    isFirstCard = false;
                }
                else
                {
                    var rawCardValue = card % CardsInOneDeck; // Determine which card, ignoring that there's multiple decks
                    rawCardValue %= CardsInASuit; // Which card in suit
                    rawCardValue += 1; // Adjust from zero-based

                    if (rawCardValue == 1)
                    {
                        aceCount++;
                        total += 11;
                    }
                    else if (rawCardValue < 10)
                    {
                        total += rawCardValue;
                    }
                    else // Royal Card
                    {
                        total += 10;
                    }
                }
            }

            if (total > 21 && aceCount > 0)
            {
                for (int i = 0; i < aceCount; i++)
                {
                    if (total > 21)
                    {
                        total -= 10; // Convert the Ace from 11 down to 1
                    }
                }
            }

            return total;
        }
    }
}
