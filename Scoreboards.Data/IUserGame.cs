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
        UserGame GetById(int userGameId);
        Task SetGameImageAsync(int userGameId, Uri uri);
        
        // Lewis added
        IEnumerable<UserGame> getUserGamesByGameId(string gameId);
        IEnumerable<UserGame> getUserGamesByUserId(string userId);
        int getWinsByIdAndGameId(string userId, string gameId);
        int getDrawsByIdAndGameId(string userId, string gameId);
        int getLosesByIdAndGameId(string userId, string gameId);
        decimal getRatioWithIdAndGameId(string userId, string gameId);
        decimal getRatioIncludingDrawWithIdAndGameId(string userId, string gameId);
        ///////////////////////////////////////

        // CRUD Operations
        Task AddUserGameAsync(UserGame userGame);
        Task Delete(int userGameId);
        Task EditUserGame(UserGame newUserGameContent);
    }
}
