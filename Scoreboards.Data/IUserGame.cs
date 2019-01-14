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
        
        // Lewis added
        IEnumerable<UserGame> getUserGameById(string userId);
        IEnumerable<UserGame> getUserGameByGameId(string gameId);
        UserGame getUserGameByGameName(string gameName);
        int getWinsById(string userId);
        int getLosesById(string userId);
        decimal getRatioWithId(string userId);
        int getWinsByIdAndGameName(string userId, string gameName);
        int getLosesByIdAndGameName(string userId, string gameName);
        decimal getRatioWithIdAndGameName(string userId, string gameName);
        ///////////////////////////////////////
        
        // CRUD Operations
        Task AddUserGameAsync(UserGame userGame);
        Task Delete(int userGameId);
        Task EditUserGame(UserGame newUserGameContent);
    }
}
