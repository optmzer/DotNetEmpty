using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Scoreboards.Models.UserGames
{
    public class UserGameListingModel
    {
        public int Id { get; set; }
        //Game played Date
        public DateTime GamePlayedOn { get; set; }

        //Players detail
        public IdentityUser User_01 { get; set; }
        public string User_01_Id { get; set; }
        public string User_01_Name { get; set; }
        public string User_01_Team { get; set; }
        public IdentityUser User_02 { get; set; }
        public string User_02_Id { get; set; }
        public string User_02_Name { get; set; }
        public string User_02_Team { get; set; }

        // Game Name
        public string GameName { get; set; }

        //Score 
        public string GameScore { get; set; }
        public int GameScoreUser01 { get; set; }
        public int GameScoreUser02 { get; set; }

        //Winner, “USER_01_Id”, “USER_02_Id”, “DRAW”
        public string Winner { get; set; }
        //Referee details. Only keep their User.Id
        public IdentityUser Referee { get; set; }
    }
}
