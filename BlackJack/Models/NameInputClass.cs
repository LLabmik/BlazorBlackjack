using System.ComponentModel.DataAnnotations;

namespace BlackJack.Models
{
    public class NameInputClass
    {
        [Required]
        public string Name { get; set; } = string.Empty;
    }
}
