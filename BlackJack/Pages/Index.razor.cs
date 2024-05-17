using BlackJack.Models;

namespace BlackJack.Pages
{
    public partial class Index
    {
        #region Constants
        const int MinimumBet = 500;
        const int CardsInOneDeck = 52;
        const int NumberOfDecksUsed = 4;
        #endregion Constants

        #region Properties
        PageStates PageState { get; set; }
        string PlayerName { get; set; } = string.Empty;
        int PlayerCash { get; set; } = 50000;
        int BetAmount { get; set; }
        string ActionMessage { get; set; } = string.Empty;
        List<int> DeckOfCards { get; set; } = new();
        int CurrentCard { get; set; }
        PlayerModel PlayerHand { get; set; } = new();
        PlayerModel DealerHand { get; set; } = new();
        NameInputClass NameModel { get; set; } = new();
        BetInputClass BetModel { get; set; } = new(MinimumBet);
        #endregion Properties

        readonly Random rnd = new();

        #region Public Methods
        void GetPlayerName()
        {
            PlayerName = NameModel.Name;

            PageState = PageStates.Bet;
            StateHasChanged();
        }

        void NewGame()
        {
            ActionMessage = string.Empty;

            PlaceBet();
        }

        void PlaceBet()
        {
            if (BetModel.Bet > PlayerCash)
            {
                BetModel.Bet = PlayerCash;
            }

            PageState = PageStates.Bet;
            StateHasChanged();
        }

        void BeginGame()
        {
            BetAmount = BetModel.Bet;

            if (PlayerCash < BetAmount)
            {
                ActionMessage = $"You don't have ${BetAmount}, you only have ${PlayerCash}.";

                PageState = PageStates.Bet;
                StateHasChanged();
            }
            else if (BetAmount < MinimumBet)
            {
                ActionMessage = $"Minimum Bet is ${MinimumBet}. Please try again...";

                PageState = PageStates.Bet;
                StateHasChanged();
            }
            else
            {
                ShuffleCards();

                PlayerCash -= BetAmount;

                ActionMessage = $"You place a bet of ${BetAmount}. You have ${PlayerCash} remaining.";

                PlayerHand = new PlayerModel // Player
                {
                    Name = PlayerName,
                    Cards = DealFirstHand(),
                    IsHidingFirstCard = false
                };

                DealerHand = new PlayerModel // Player
                {
                    Name = "DEALER",
                    Cards = DealFirstHand(),
                    IsHidingFirstCard = true
                };

                if (PlayerHand.Score() == 21)
                {
                    Stand();
                }
                else
                {
                    PageState = PageStates.StartGame;
                    StateHasChanged();
                }

                PageState = PageStates.StartGame;
                StateHasChanged();
            }
        }

        void Hit()
        {
            PlayerHand.Cards.Add(NextCard());

            if (PlayerHand.Score() > 21)
            {
                Stand();
            }
            else
            {
                PageState = PageStates.Hit;
                StateHasChanged();
            }
        }

        void Stand()
        {
            DealerHand.IsHidingFirstCard = false;
            ProcessDealerHand();

            var dealerScore = DealerHand.Score();
            var playerScore = PlayerHand.Score();

            if (dealerScore > 21 && playerScore > 21)
            {
                ActionMessage = $"You both busted! You lose your bet of ${BetAmount}.";
            }
            else if (dealerScore > 21)
            {
                PlayerCash += BetAmount;

                ActionMessage = $"Dealer Busts, You Win ${BetAmount}!";
            }
            else if (playerScore > 21)
            {
                ActionMessage = $"You Bust! You lose your bet of ${BetAmount}.";
            }
            else if (dealerScore == playerScore)
            {
                ActionMessage = "Push, You Both Tied! You get your Bet back.";

                PlayerCash += BetAmount;
            }
            else if (dealerScore > playerScore)
            {
                ActionMessage = $"Dealer Wins! You lose your bet of ${BetAmount}.";
            }
            else
            {
                if (playerScore == 21 && PlayerHand.Cards.Count == 2)
                {
                    int winnings = BetAmount * 2;
                    PlayerCash += winnings;

                    ActionMessage = $"BlackJack! You win ${winnings}!! (200% of Bet)";
                }
                else
                {
                    PlayerCash += (int)(BetAmount * 1.5);

                    ActionMessage = $"You Win ${(int)(BetAmount * 1.5)}! (150% of Bet)";
                }
            }

            PageState = PageStates.Stand;
            StateHasChanged();
        }
        #endregion Public Methods

        #region Private Methods
        private void ShuffleCards()
        {
            DeckOfCards = new List<int>();

            // Generate deck with 4 complete sets of cards
            for (int i = 0; i < CardsInOneDeck * NumberOfDecksUsed; i++)
            {
                DeckOfCards.Add(i);
            }

            int n = DeckOfCards.Count;
            while (n > 1)
            {
                n--;
                int k = rnd.Next(n + 1);
                (DeckOfCards[n], DeckOfCards[k]) = (DeckOfCards[k], DeckOfCards[n]);
            }
        }

        private int NextCard()
        {
            var retVal = DeckOfCards[CurrentCard];

            CurrentCard++;
            if (CurrentCard > (CardsInOneDeck * NumberOfDecksUsed) - 1)
            {
                ShuffleCards();
            }

            return retVal;
        }

        private List<int> DealFirstHand()
        {
            var cardsInHand = new List<int>();

            var cardCount = 2;

            for (int i = 0; i < cardCount; i++)
            {
                cardsInHand.Add(NextCard());
            }

            return cardsInHand;
        }

        private void ProcessDealerHand()
        {
            bool dealerRests = false;
            DealerHand.IsHidingFirstCard = false;

            do
            {
                if (DealerHand.Score() > 21)
                {
                    // Dealer Busts
                    dealerRests = true;
                }
                else
                {
                    if (DealerHand.Score() < 17)
                    {
                        DealerHand.Cards.Add(NextCard());
                    }
                    else // Dealer rests at 17
                    {
                        dealerRests = true;
                    }
                }
            } while (!dealerRests);
        }

        private string CardImageFilename(int cardNum)
        {
            var card = (cardNum % 13) + 1;
            string cardName = card.ToString();

            switch (card)
            {
                case 1:
                    cardName = "a"; // Ace
                    break;
                case 10:
                    cardName = "t"; // Ten
                    break;
                case 11:
                    cardName = "j"; // Jack
                    break;
                case 12:
                    cardName = "q"; // Queen
                    break;
                case 13:
                    cardName = "k"; // King
                    break;
                default: // Card between 2 & 9, do not modify cardName
                    break;
            }

            switch (cardNum / 13)
            {
                case 0:
                    cardName += "h"; // Heart
                    break;
                case 1:
                    cardName += "d"; // Diamond
                    break;
                case 2:
                    cardName += "c"; // Club
                    break;
                case 3:
                    cardName += "s"; // Spade
                    break;
            }

            return $"images/cards/{cardName}.svg";
        }
        #endregion Private Methods

        #region Enums
        enum PageStates
        {
            Default,
            Bet,
            StartGame,
            Hit,
            Stand
        }
        #endregion Enums
    }
}
