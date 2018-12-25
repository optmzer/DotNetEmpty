using Scoreboards.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Scoreboards.Data
{
    public interface IUserGame
    {
        IEnumerable<UserGame> GetAll();
        IEnumerable<UserGame> GetLatest(int howMany);
        IEnumerable<UserGame> GetGamesByGameName(string gameName);
        UserGame GetById(int userGameId);
        Task SetGameImageAsync(int userGameId, Uri uri);

        // CRUD Operations
        Task AddUserGameAsync(UserGame userGame);
        Task Delete(int userGameId);
        Task EditUserGame(UserGame newUserGameContent);
    }
}
