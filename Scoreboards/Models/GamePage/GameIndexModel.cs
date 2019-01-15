using Scoreboards.Data.Models;
using Scoreboards.Models.UserGames;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Scoreboards.Models.GamePage
{
    public class GameIndexModel
    {
        public IEnumerable<Game> gameList { get; set; }
    }
}
