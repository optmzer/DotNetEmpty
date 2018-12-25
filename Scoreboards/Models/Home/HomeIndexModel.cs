using Scoreboards.Models.UserGames;
using System.Collections.Generic;

namespace Scoreboards.Models.Home
{
    public class HomeIndexModel
    {
        public IEnumerable<UserGameListingModel> LatestGames { get; set; }
        public string SearchQuery { get; set; }
    }
}
