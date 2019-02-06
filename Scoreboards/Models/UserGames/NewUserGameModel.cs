using Scoreboards.Data.Models;
using Scoreboards.Data.Validators;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Scoreboards.Models.UserGames
{
    public class NewUserGameModel
    {
        public string UserGameId { get; set; }

        public IEnumerable<Game> GamesList { get; set; }
        public IEnumerable<ApplicationUser> UsersList { get; set; }

        //Players detail
        [Required]
        [NotEqual("User_02_Id", ErrorMessage = "Player 1 and 2 should be different")]
        public string User_01_Id { get; set; }

        [Required]
        public string User_01_Team { get; set; }

        [Required]
        [NotEqual("User_01_Id", ErrorMessage = "Player 1 and 2 should be different")]
        public string User_02_Id { get; set; }

        [Required]
        public string User_02_Team { get; set; }

        //Score 
        //public string GameScore { get; set; }
        [Required]
        public int GameScoreUser01 { get; set; }

        [Required]
        public int GameScoreUser02 { get; set; }

        //Winner, “USER_01_Id”, “USER_02_Id”, “DRAW”
        public string Winner { get; set; }

        //Referee details. Only keep their User.Id
        public string RefereeUserId { get; set; }

        // Name of the game
        public string GamePlayedName { get; set; }


        // User_01_Awarder_Points
        public string User_01_Awarder_Points { get; set; }

        // User_02_Awarder_Points
        public string User_02_Awarder_Points { get; set; }
    }
}
