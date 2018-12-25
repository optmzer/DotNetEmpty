using System.ComponentModel.DataAnnotations;

namespace Scoreboards.Data.Models
{
    public class Game
    {
        public int Id { get; set; }
        [Required]
        public string GameName { get; set; }
        public string GameLogo { get; set; }
    }
}
