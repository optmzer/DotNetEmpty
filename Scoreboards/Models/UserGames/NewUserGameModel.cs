using Scoreboards.Data.Models;
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
        public string User_01_Id { get; set; }
        public string User_01_Team { get; set; }
        public string User_02_Id { get; set; }
        public string User_02_Team { get; set; }

        //Score 
        //public string GameScore { get; set; }
        [Range(0, 200, ErrorMessage = "Please enter integers in a range of {1} to {2}" )]
        public int GameScoreUser01 { get; set; }
        [Range(0, 200, ErrorMessage = "Please enter integers in a range of {1} to {2}")]
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
