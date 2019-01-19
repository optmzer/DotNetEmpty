using System.ComponentModel.DataAnnotations;

namespace Scoreboards.Data.Models
{
    public class Game
    {
        public int Id { get; set; }
        [Required]
        public string GameName { get; set; }
        public string GameLogo { get; set; }
        // Base point for every win in this game
        [Required]
        public int WinPoints { get; set; }
        // Base point for every draw in this game
        [Required]
        public int DrawPoints { get; set; }
        // Base point for every loss in this game
        [Required]
        public int LossPoints { get; set; }
    }
}
