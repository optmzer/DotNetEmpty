using Scoreboards.Data.Models;
using System.Collections.Generic;

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
        public bool IsProfileDeleted { get; set; }
        public int GameId { get; set; }
        public IEnumerable<MonthlyWinners> MonthlyWins { get; set; }
    }
}
