using Scoreboards.Data.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Scoreboards.Data
{
    public interface IGame
    {
        Game GetById(int gameId);
        Game GetByName(string gameName);
        Task SetGameImageAsync(string uri);
        Task SetGameNameAsync(string gameName);
        Task AddGame(Game game);
        IEnumerable<Game> GetAll();
    }
}
