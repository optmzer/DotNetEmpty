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
        public int[] CalculatePoints(int flatPoints, decimal multiplier, int flatLoss, decimal lossMultiplier, string user1Id, string user2Id, string winner, string gameId)
        {
            var noOfGamePlayed1 = getTotalGamePlayedByUserId(user1Id);
            var noOfGamePlayed2 = getTotalGamePlayedByUserId(user2Id);
            var user_01_Points = getUserPoint(user1Id, gameId);
            var user_02_Points = getUserPoint(user2Id, gameId);

            var multiplierModified = (decimal)0;
            var modifiedLoss = (decimal)0;
            if (user_01_Points + user_02_Points <= 100)
            {
                multiplierModified = Math.Round(Math.Abs(user_01_Points - user_02_Points) / (decimal)100.0 * multiplier);
                modifiedLoss = Math.Round(Math.Abs(user_01_Points - user_02_Points) / (decimal)100.0 * lossMultiplier);
            }
            else
            {
                multiplierModified = Math.Round((Math.Abs(user_02_Points - user_01_Points) / (decimal)(user_02_Points + user_01_Points)) * multiplier);
                modifiedLoss = Math.Round((Math.Abs(user_02_Points - user_01_Points) / (decimal)(user_02_Points + user_01_Points)) * lossMultiplier);

            }

            int[] calculatedPointsChange;

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
                        calculatedPointsChange = new int[2] { flatPoints, -flatLoss };
                    }
                    else
                    {
                        calculatedPointsChange = new int[2] { -flatLoss, flatPoints };
                    }
                }
                else if (user_01_Points > user_02_Points)
                {
                    if (winner == user1Id)
                    {
                        calculatedPointsChange = new int[2] { (int)(flatPoints - multiplierModified), (int)(-flatLoss + modifiedLoss) };
                    }
                    else
                    {
                        calculatedPointsChange =  new int[2] { (int)(-flatLoss - modifiedLoss), (int)(flatPoints + multiplierModified) };
                    }
                }
                else
                {
                    if (winner == user1Id)
                    {
                        calculatedPointsChange =  new int[2] { (int)(flatPoints + multiplierModified), (int)(-flatLoss - modifiedLoss) };
                    }
                    else
                    {
                        calculatedPointsChange =  new int[2] { (int)(-flatLoss + modifiedLoss), (int)(flatPoints - multiplierModified) };
                    }
                }
            }

            return CheckForInvalidReduction(CheckForGamesPlayed(calculatedPointsChange, noOfGamePlayed1, noOfGamePlayed2), user_01_Points, user_02_Points);
        }
        /**
         * This method checks that the scores provided will not reduce either players points below 15 and updates
         * the score to end with 15 points if it does.
         * 
         */
        public int[] CheckForInvalidReduction(int[] points, int user1Points, int user2Points)
        {
            // Sets the minimum number a points a player can fall to.
            int minimumPoints = 15;
            int[] updatedPointsChange = points;
            // Checks the user won't end below 15 points
            if (user1Points+points[0] < minimumPoints)
            {
                // Considers the case when a user has never won a game (still on 0 points)
                if (user1Points < minimumPoints)
                {
                    // This is the case where the user has drawn and the points gained doesn't put him over 15.
                    if (points[0] > 0)
                    {
                        updatedPointsChange[0] = points[0];
                    }
                    else
                    {
                        updatedPointsChange[0] = 0;
                    }
                }
                else
                {
                    updatedPointsChange[0] = -user1Points + minimumPoints;
                }
            }
            else if (user2Points+points[1] < minimumPoints)
            {
                if (user2Points < minimumPoints)
                {
                    if (points[1] > 0)
                    {
                        updatedPointsChange[1] = points[1];
                    }
                    else
                    {
                        updatedPointsChange[1] = 0;
                    }
                }
                else
                {
                    updatedPointsChange[1] = -user2Points + minimumPoints;
                }
            }
            return updatedPointsChange;
        }

        /**
         * Checks the users have both played more than 5 gameand if they haven't returns updated points change
         * which contains no points loss.
        */
        public int[] CheckForGamesPlayed(int[] points, int gamesPlayed1, int gamesPlayed2)
        {
            if (points[0] < 0 && gamesPlayed1 < 5)
            {
                points[0] = 0;
            }
            else if (points[1] < 0 && gamesPlayed2 < 5)
            {
                points[1] = 0;
            }
            return points;
        }

        public int getUserPoint(string userId, string gameId)
        {
            if (gameId == null || gameId == "")
            {
                var listOfUserGames = getUserGamesByUserId(userId);
                var point1 = listOfUserGames.Where(game => game.User_01_Id == userId).Sum(game => game.User_01_Awarder_Points);
                var point2 = listOfUserGames.Where(game => game.User_02_Id == userId).Sum(game => game.User_02_Awarder_Points);
                return point1 + point2;
            }
            else
            {
                var listOfUserGames = getUserGamesByUserId(userId);
                var point1 = listOfUserGames.Where(game => game.User_01_Id == userId && game.GamePlayed.Id.ToString() == gameId).Sum(game => game.User_01_Awarder_Points);
                var point2 = listOfUserGames.Where(game => game.User_02_Id == userId && game.GamePlayed.Id.ToString() == gameId).Sum(game => game.User_02_Awarder_Points);
                return point1 + point2;
            }
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
