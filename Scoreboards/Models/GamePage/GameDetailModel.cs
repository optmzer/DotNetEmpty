using Scoreboards.Data.Models;
using Scoreboards.Models.Home;
using Scoreboards.Models.UserGames;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Scoreboards.Models.GamePage
{
    public class GameDetailModel
    {
        public Game GameDetail { get; set; }
        public IEnumerable<UserGameListingModel> GameSpecificMatchHistory { get; set; }
    }
}
