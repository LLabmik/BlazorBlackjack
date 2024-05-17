using System.ComponentModel.DataAnnotations;

namespace BlackJack.Models
{
    public class BetInputClass
    {
        public BetInputClass(int minimumBet) { Bet = minimumBet; }

        [Required]
        public int Bet { get; set; }
    }
}
