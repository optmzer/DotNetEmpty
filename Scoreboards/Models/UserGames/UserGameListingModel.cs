using Microsoft.AspNetCore.Identity;
using Scoreboards.Data.Models;
using Scoreboards.Data.Validators;
using System;
using System.ComponentModel.DataAnnotations;

namespace Scoreboards.Models.UserGames
{
    public class UserGameListingModel
    {
        public int Id { get; set; }
        //Game played Date
        public DateTime GamePlayedOn { get; set; }

        //Players detail
        public ApplicationUser User_01 { get; set; }

        [Required]
        [NotEqual("User_02_Id", ErrorMessage = "Player 1 and 2 should be different")]
        public string User_01_Id { get; set; }

        [Required]
        public string User_01_Name { get; set; }
        public string User_01_Team { get; set; }
        public ApplicationUser User_02 { get; set; }

        [Required]
        [NotEqual("User_01_Id", ErrorMessage = "Player 1 and 2 should be different")]
        public string User_02_Id { get; set; }
        public string User_02_Name { get; set; }

        [Required]
        public string User_02_Team { get; set; }

        // Game Name
        [Required]
        public string GameName { get; set; }

        [Required]
        public int GamePlayedId { get; set; }

        public Boolean NeedToApologise { get; set; }

        public Boolean Apologised { get; set; }

        //Score 
        public string GameScore { get; set; }

        [Required]
        [Range(0, 500, ErrorMessage = "Please enter integers in a range of {1} to {2}")]
        public int GameScoreUser01 { get; set; }

        [Required]
        [Range(0, 500, ErrorMessage = "Please enter integers in a range of {1} to {2}")]
        public int GameScoreUser02 { get; set; }

        //Winner, “USER_01_Id”, “USER_02_Id”, “DRAW”
        public string Winner { get; set; }
        //Referee details. Only keep their User.Id
        public IdentityUser Referee { get; set; }
    }
}
