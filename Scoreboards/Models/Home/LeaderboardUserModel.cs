using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Scoreboards.Models.Home
{
    public class LeaderboardUserModel
    {
        public string UserId { get; set; }
        public string ProfileImageUrl { get; set; }
        public string UserName { get; set; }
        public string Wins { get; set; }
        public string Draws { get; set; }
        public string Loses { get; set; }
        public string Ratio { get; set; }
        public string Points { get; set; }
    }
}
