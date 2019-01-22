using Microsoft.EntityFrameworkCore;
using Scoreboards.Data;
using Scoreboards.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

/*
 * Updates UserGame data
 */
namespace Scoreboards.Services
{
    public class UserGameService : IUserGame
    {
        private readonly ApplicationDbContext _context;
        public UserGameService(ApplicationDbContext context)
        {
            _context = context;
        }

        public IEnumerable<UserGame> GetAll()
        {
            return _context.UserGames.Include(game => game.GamePlayed);
        }

        public IEnumerable<UserGame> GetLatest(int numberOfGames)
        {
            return GetAll()
                .OrderByDescending(userGame => userGame.GamePlayedOn)
                .Take(numberOfGames);
        }

        // Lewis added
        public int getWinsByIdAndGameId(string userId, string gameId)
        {
            // if gameName is null, provide overall
            // else provide overall
            if (gameId == null || gameId == "")
            {
                return GetAll().Where(userGame => (userGame.User_01_Id == userId || userGame.User_02_Id == userId) && userGame.Winner == userId).Count();
            } else
            {
                return GetAll().Where(userGame => ((userGame.User_01_Id == userId || userGame.User_02_Id == userId) && userGame.Winner == userId) && gameId == userGame.GamePlayed.Id.ToString()).Count();
            }
        }
        public int getDrawsByIdAndGameId(string userId, string gameId)
        {
            // if gameName is null, provide overall
            // else provide overall
            if (gameId == null || gameId == "")
            {
                return GetAll().Where(userGame => (userGame.User_01_Id == userId || userGame.User_02_Id == userId) && userGame.Winner.ToLower() == "draw").Count();
            }
            else
            {
                return GetAll().Where(userGame => ((userGame.User_01_Id == userId || userGame.User_02_Id == userId) && userGame.Winner.ToLower() == "draw") && gameId == userGame.GamePlayed.Id.ToString()).Count();
            }
        }
        public int getLosesByIdAndGameId(string userId, string gameId)
        {
            // if gameName is null, provide overall
            // else provide overall
            if (gameId == null || gameId == "")
            {
                return GetAll().Where(userGame => (userGame.User_01_Id == userId || userGame.User_02_Id == userId) && userGame.Winner != userId && userGame.Winner.ToLower() != "draw").Count();
            }
            else
            {
                return GetAll().Where(userGame => ((userGame.User_01_Id == userId || userGame.User_02_Id == userId) && userGame.Winner != userId && userGame.Winner.ToLower() != "draw") && gameId == userGame.GamePlayed.Id.ToString()).Count();
            }
        }
        public decimal getRatioWithIdAndGameId(string userId, string gameId)
        {
            int wins = getWinsByIdAndGameId(userId, gameId);
            int loses = getLosesByIdAndGameId(userId, gameId);
            int total = wins + loses;
            if (total == 0)
            {
                return 0;
            }
            else
            {
                decimal ratio = (decimal)wins / (decimal)(total) * 100;
                return Math.Round(ratio, 2);
            }
        }
        public decimal getRatioIncludingDrawWithIdAndGameId(string userId, string gameId)
        {
            int wins = getWinsByIdAndGameId(userId, gameId);
            int draws = getDrawsByIdAndGameId(userId, gameId);
            int loses = getLosesByIdAndGameId(userId, gameId);
            int total = wins + draws + loses;
            if (total == 0)
            {
                return 0;
            }
            else
            {
                decimal ratio = (decimal)wins / (decimal)(total) * 100;
                return Math.Round(ratio, 2);
            }
        }
        public IEnumerable<UserGame> getUserGamesByGameId(string gameId)
        {
            if (gameId==null||gameId=="")
            {
                return GetAll();
            } else
            {
                return GetAll().Where(userGame => (userGame.GamePlayed.Id.ToString() == gameId));
            }
            
        }

        public IEnumerable<UserGame> getUserGamesByUserId(string userId)
        {
            return GetAll()
                .Where(userGame => 
                    (userGame.User_01_Id.ToString() == userId) 
                    || userGame.User_02_Id.ToString() == userId);
        }
        public int getTotalGamePlayedByUserId(string userId)
        {
            return GetAll()
                .Where(userGame =>
                    (userGame.User_01_Id.ToString() == userId)
                    || userGame.User_02_Id.ToString() == userId).Count();
        }

        ///////////////////////////////////////////
        public int[] CalculatePoints(int flatPoints, decimal multiplier, string user1Id, string user2Id, string winner)
        {
            var noOfGamePlayed1 = getTotalGamePlayedByUserId(user1Id);
            var noOfGamePlayed2 = getTotalGamePlayedByUserId(user2Id);
            var user_01_Points = getUserPoint(user1Id);
            var user_02_Points = getUserPoint(user2Id);

            var multiplierModified = (decimal)0;
            if (user_01_Points + user_02_Points <= 100)
            {
                multiplierModified = Math.Round(Math.Abs(user_01_Points - user_02_Points) / (decimal)100.0 * multiplier);
            }
            else
            {
                multiplierModified = Math.Round(Math.Abs(user_02_Points - user_01_Points) / (user_02_Points + user_01_Points) * multiplier);
            }

            if (winner.ToLower() == "draw")
            {//in case the game is "draw"
                if (user_01_Points > user_02_Points)
                {
                    return new int[2] { 0, (int)multiplierModified };
                }
                else if (user_01_Points < user_02_Points)
                {
                    return new int[2] { (int)multiplierModified, 0 };
                }
                else
                {
                    return new int[2] { 0, 0 };
                }
            }
            else
            {// in case game is not "draw"
                if (user_01_Points == user_02_Points)
                {
                    if (winner == user1Id)
                    {
                        if (noOfGamePlayed2 < 5)
                        {// played less than 5
                            return new int[2] { flatPoints, 0 };
                        }
                        else
                        {// played more than 5
                            return new int[2] { flatPoints, -flatPoints };
                        }
                    }
                    else
                    {
                        if (noOfGamePlayed1 < 5)
                        {// played less than 5
                            return new int[2] { 0, flatPoints };
                        }
                        else
                        {// played more than 5
                            return new int[2] { -flatPoints, flatPoints };
                        }
                    }
                }
                else if (user_01_Points > user_02_Points)
                {
                    if (winner == user1Id)
                    {
                        if (noOfGamePlayed2 < 5)
                        {// played less than 5
                            return new int[2] { (int)(flatPoints - multiplierModified), 0 };
                        }
                        else
                        {
                            return new int[2] { (int)(flatPoints - multiplierModified), (int)(-flatPoints + multiplierModified) };
                        }
                    }
                    else
                    {
                        if (noOfGamePlayed1 < 5)
                        {// played less than 5
                            return new int[2] { 0, (int)(flatPoints + multiplierModified) };
                        }
                        else
                        {
                            return new int[2] { (int)(-flatPoints - multiplierModified), (int)(flatPoints + multiplierModified) };
                        }
                    }
                }
                else
                {
                    if (winner == user1Id)
                    {
                        if (noOfGamePlayed2 < 5)
                        {// played less than 5
                            return new int[2] { (int)(flatPoints + multiplierModified), 0 };
                        }
                        else
                        {
                            return new int[2] { (int)(flatPoints + multiplierModified), (int)(-flatPoints - multiplierModified) };
                        }
                    }
                    else
                    {
                        if (noOfGamePlayed1 < 5)
                        {// played less than 5
                            return new int[2] { 0, (int)(flatPoints - multiplierModified) };
                        }
                        else
                        {
                            return new int[2] { (int)(-flatPoints + multiplierModified), (int)(flatPoints - multiplierModified) };
                        }
                    }
                }
            }
        }
        public int getUserPoint(string userId)
        {
            var listOfUserGames = getUserGamesByUserId(userId);
            var point1 = listOfUserGames.Where(game => game.User_01_Id == userId).Sum(game => game.User_01_Awarder_Points);
            var point2 = listOfUserGames.Where(game => game.User_02_Id == userId).Sum(game => game.User_02_Awarder_Points);
            return point1 + point2;
        }
        public UserGame GetById(int userGameId)
        {
            return GetAll().FirstOrDefault(userGame => userGame.Id == userGameId);
        }

        public async Task AddUserGameAsync(UserGame userGame)
        {
            /**
            * Entity framwork handls all logic for us
            * all we need to do is to call _context.Add() method
            * and EntityFramwork will figure out where to stick it.
            */
            _context.Add(userGame);
            await _context.SaveChangesAsync(); // commits changes to DB.
        }

        public async Task DeleteUserGame(int userGameId)
        {
            var userGame = GetById(userGameId);

            _context.Remove(userGame);
            await _context.SaveChangesAsync(); // commits changes to DB.
        }

        public async Task EditUserGame(UserGame newUserGameContent)
        {
            var game = GetById(newUserGameContent.Id);
            // Mark for update
            _context.Entry(game).State = EntityState.Modified;
            // Update values
            game.GameScoreUser01 = newUserGameContent.GameScoreUser01;
            game.GameScoreUser02 = newUserGameContent.GameScoreUser02;

            game.User_01_Team = newUserGameContent.User_01_Team;
            game.User_02_Team = newUserGameContent.User_02_Team;

            game.Winner = newUserGameContent.Winner;

            await _context.SaveChangesAsync();
        }

        public Task SetGameImageAsync(int userGameId, Uri uri)
        {
            throw new NotImplementedException();
        }

        public int CountWinsForUser(string userId)
        {
            return GetAll().Where(uGame => uGame.Winner == userId).Count();
        }

    }
}
