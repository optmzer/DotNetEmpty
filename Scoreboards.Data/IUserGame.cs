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
        int getWinsByIdAndGameId(IEnumerable<UserGame> preparedData, string userId, string gameId);
        int getDrawsByIdAndGameId(IEnumerable<UserGame> preparedData, string userId, string gameId);
        int getLosesByIdAndGameId(IEnumerable<UserGame> preparedData, string userId, string gameId);
        decimal getRatioWithIdAndGameId(int wins, int losses);
        decimal getRatioWithIdAndGameId(string userId, string gameId);
        decimal getRatioWithIdAndGameId(IEnumerable<UserGame> userGameList, string userId, string gameId);
        decimal getRatioIncludingDrawWithIdAndGameId(string userId, string gameId);
        int getUserPoint(string userId, string gameId);
        int getUserPoint(IEnumerable<UserGame> userSpecificUGList, string userId, string gameId);
        int getTotalGamePlayedByUserId(string userId);
        int[] CalculatePoints(int flatPoints, decimal multiplier, int flatLoss, decimal lossMultiplier, string user1Id, string user2Id, string winner, string gameId);
        ///////////////////////////////////////

        // CRUD Operations
        Task AddUserGameAsync(UserGame userGame);
        Task DeleteUserGame(int userGameId);
        Task EditUserGame(UserGame newUserGameContent);
    }
}
