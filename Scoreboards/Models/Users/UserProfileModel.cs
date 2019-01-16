using Scoreboards.Data;
using Scoreboards.Data.Models;
using Scoreboards.Models.Home;
using Scoreboards.Models.UserGames;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Scoreboards.Models.Users
{
    public class UserProfileModel
    {
        public IEnumerable<UserGameListingModel> UsersGames { get; set; }
        public Dictionary<string, LeaderboardUserModel> GameStatistcs { get; set; }
        public ApplicationUser User { get; set; }

    }
}
