using Scoreboards.Models.UserGames;
using System.Collections.Generic;

namespace Scoreboards.Models.Home
{
    public class HomeIndexModel
    {
        // lewis added
        public IEnumerable<LeaderboardUserModel> UsersData { get; set; }
        public List<SelectListItem> DropDownData { get; set; }
        public string itemSelected { get; set; }
        //////////////
        
        public IEnumerable<UserGameListingModel> LatestGames { get; set; }
        public string SearchQuery { get; set; }
    }
}
