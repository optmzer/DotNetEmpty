using Scoreboards.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Scoreboards.Data
{
    public interface IGame
    {
        Game GetById(int gameId);
        Game GetByName(string gameName);
        Task SetGameImageAsync(string userId, Uri uri);
        Task SetGameNameAsync(string userId, string gameName);
        IEnumerable<Game> GetAll();
    }
}
