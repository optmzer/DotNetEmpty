using System;
using System.ComponentModel.DataAnnotations;

namespace Scoreboards.Data.Models
{
    public class UserGame
    {
        public int Id { get; set; }
        //Game played Date
        public DateTime GamePlayedOn { get; set; } = DateTime.Now;
        //Players detail
        [Required]
        public string User_01_Id { get; set; }
        [Required]
        public string User_01_Team { get; set; }
        [Required]
        public string User_02_Id { get; set; }
        [Required]
        public string User_02_Team { get; set; }

        // Check for if the user has apologised (for FIFA)
        public Boolean Apologised { get; set; } = false;

        [Required]
        public int GameScoreUser01 {get; set;}
        [Required]
        public int GameScoreUser02 { get; set; }

        //Winner, “USER_01_Id”, “USER_02_Id”, “DRAW”
        public string Winner { get; set; }

        //Referee details. Only keep their User.Id
        public string RefereeUserId { get; set; }

        //For future use. Comments 
        public string SubmissionComments { get; set; } = "The winner takes it all";
        
        // Name of the game
        public virtual Game GamePlayed { get; set; }

        // User points for this game
        public int User_01_Awarder_Points { get; set; }
        public int User_02_Awarder_Points { get; set; }
    }
}
