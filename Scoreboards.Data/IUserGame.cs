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
        IEnumerable<UserGame> GetAll(DateTime monthFetched);
        IEnumerable<UserGame> GetLatest(int howMany);
        UserGame GetById(int userGameId);

        // Lewis added
        IEnumerable<UserGame> getUserGamesByGameId(string gameId);
        IEnumerable<UserGame> getUserGamesByUserId(string userId);
        IEnumerable<UserGame> getUserGamesByGameIdAndMonth(string gameId, DateTime monthFetched);
        int getWinsByIdAndGameId(IEnumerable<UserGame> preparedData, string userId, string gameId);
        int getDrawsByIdAndGameId(IEnumerable<UserGame> preparedData, string userId, string gameId);
        int getLosesByIdAndGameId(IEnumerable<UserGame> preparedData, string userId, string gameId);
        decimal calculateWinRatio(int wins, int losses);
        decimal getRatioWithIdAndGameId(IEnumerable<UserGame> userGameList, string userId, string gameId);
        decimal getRatioIncludingDrawWithIdAndGameId(string userId, string gameId);
        int getUserPoint(IEnumerable<UserGame> userSpecificUGList, string userId, string gameId);
        int getUserPointsByMonth(string userId, string gameId);
        int getTotalGamePlayedByUserId(string userId);
        int[] CalculatePoints(int user_01_Points, int user_02_Points, string winner, string gameId);
        string GetLastMonthWinner(string gameId);


        // CRUD Operations
        Task AddUserGameAsync(UserGame userGame);
        Task DeleteUserGame(int userGameId);
        Task DeleteUserGameByMonth(DateTime month);
        Task DeleteAllUserGames();
        Task EditUserGame(UserGame newUserGameContent);
        Task DeleteUserGamesForGame(Game game);
        Task updateGameApology(int userGameId);

    }
}
