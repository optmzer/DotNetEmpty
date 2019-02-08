using Scoreboards.Data.Models;
using Scoreboards.Models.UserGames;
using System.Collections.Generic;

namespace Scoreboards.Models.GamePage
{
    public class GameDetailModel
    {
        public Game GameDetail { get; set; }
        public IEnumerable<UserGameListingModel> GameSpecificMatchHistory { get; set; }
        public string ReigningChampion { get; set; }
    }
}
