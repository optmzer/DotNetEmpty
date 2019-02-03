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
        private readonly IMonthlyWinners _monthlyWinnersService;
        public UserGameService(ApplicationDbContext context, IApplicationUser userService, IMonthlyWinners monthlyWinnersService)
        {
            _context = context;
            _monthlyWinnersService = monthlyWinnersService;
        }

        /**
         * Returns a list of all user games in the database
         */
        public IEnumerable<UserGame> GetAll()
        {
            return _context.UserGames.Include(game => game.GamePlayed);
        }
        public IEnumerable<UserGame> GetAll(DateTime monthFetched)
        {
            return _context.UserGames.Include(game => game.GamePlayed).Where(game=> game.GamePlayedOn.Month == monthFetched.Month && game.GamePlayedOn.Year == monthFetched.Year);
        }
        /**
         * Gets the latest games, the number returned is specified in the input
         */
        public IEnumerable<UserGame> GetLatest(int numberOfGames)
        {
            return GetAll()
                .OrderByDescending(userGame => userGame.GamePlayedOn)
                .Take(numberOfGames);
        }
        
        /**
         * Returns the number of wins an input user has in a specified game
         * preparedData is used to optimise the algorithm by avoiding excessive calls to GetAll()
         */
        public int getWinsByIdAndGameId(IEnumerable<UserGame> preparedData, string userId, string gameId)
        {
            // if preparedData is provided, no need to GetAll
            if (preparedData == null)
            {
                // if game Id is null, provide overall
                // else provide specified game wins
                if (gameId == null || gameId == "" || gameId == "0")
                {
                    return GetAll().Where(userGame => 
                                          (userGame.User_01_Id == userId || userGame.User_02_Id == userId) 
                                          && userGame.Winner == userId).Count();
                }
                else
                {
                    return GetAll().Where(userGame => 
                                          ((userGame.User_01_Id == userId || userGame.User_02_Id == userId) 
                                          && userGame.Winner == userId) 
                                          && gameId == userGame.GamePlayed.Id.ToString()).Count();
                }
            }
            else
            {
                // if game Id is null, provide overall
                // else provide specified game wins
                if (gameId == null || gameId == "" || gameId== "0")
                {
                    return preparedData.Where(userGame => 
                                              (userGame.User_01_Id == userId || userGame.User_02_Id == userId) 
                                              && userGame.Winner == userId).Count();
                }
                else
                {
                    return preparedData.Where(userGame => 
                                              ((userGame.User_01_Id == userId || userGame.User_02_Id == userId) 
                                              && userGame.Winner == userId) 
                                              && gameId == userGame.GamePlayed.Id.ToString()).Count();
                }
            }
        }

        /**
         * Returns the number of Draws an input user has in a specified game
         * preparedData is used to optimise the algorithm by avoiding excessive calls to GetAll()
         */
        public int getDrawsByIdAndGameId(IEnumerable<UserGame> preparedData, string userId, string gameId)
        {
            if (preparedData == null)
            {
                // if gameId is null, provide overall
                // else provide the number of draws for the specified game
                if (gameId == null || gameId == "" || gameId == "0")
                {
                    return GetAll().Where(userGame => 
                                          (userGame.User_01_Id == userId || userGame.User_02_Id == userId) 
                                          && userGame.Winner.ToLower() == "draw").Count();
                }
                else
                {
                    return GetAll().Where(userGame => 
                                          ((userGame.User_01_Id == userId || userGame.User_02_Id == userId) 
                                          && userGame.Winner.ToLower() == "draw") 
                                          && gameId == userGame.GamePlayed.Id.ToString()).Count();
                }
            }
            else
            {
                // if gameId is null, provide overall
                // else provide the number of draws for the specified game
                if (gameId == null || gameId == "" || gameId == "0")
                {
                    return preparedData.Where(userGame => 
                                              (userGame.User_01_Id == userId || userGame.User_02_Id == userId) 
                                              && userGame.Winner.ToLower() == "draw").Count();
                }
                else
                {
                    return preparedData.Where(userGame => 
                                              ((userGame.User_01_Id == userId || userGame.User_02_Id == userId) 
                                              && userGame.Winner.ToLower() == "draw") 
                                              && gameId == userGame.GamePlayed.Id.ToString()).Count();
                }
            }
        }

        /**
         * Returns the number of Losses an input user has in a specified game
         * preparedData is used to optimise the algorithm by avoiding excessive calls to GetAll()
         */
        public int getLosesByIdAndGameId(IEnumerable<UserGame> preparedData, string userId, string gameId)
        {
            if (preparedData == null)
            {
                // if gameId is null, provide overall
                // else provide the number of losses for the specified game
                if (gameId == null || gameId == "" || gameId == "0")
                {
                    return GetAll().Where(userGame => 
                                          (userGame.User_01_Id == userId || userGame.User_02_Id == userId) 
                                          && userGame.Winner != userId 
                                          && userGame.Winner.ToLower() != "draw").Count();
                }
                else
                {
                    return GetAll().Where(userGame => 
                                          ((userGame.User_01_Id == userId || userGame.User_02_Id == userId) 
                                          && userGame.Winner != userId 
                                          && userGame.Winner.ToLower() != "draw") 
                                          && gameId == userGame.GamePlayed.Id.ToString()).Count();
                }
            }
            else
            {
                // if gameId is null, provide overall
                // else provide the number of losses for the specified game
                if (gameId == null || gameId == "" || gameId == "0")
                {
                    return preparedData.Where(userGame => 
                                              (userGame.User_01_Id == userId || userGame.User_02_Id == userId) 
                                              && userGame.Winner != userId 
                                              && userGame.Winner.ToLower() != "draw").Count();
                }
                else
                {
                    return preparedData.Where(userGame => 
                                              ((userGame.User_01_Id == userId || userGame.User_02_Id == userId) 
                                              && userGame.Winner != userId 
                                              && userGame.Winner.ToLower() != "draw") 
                                              && gameId == userGame.GamePlayed.Id.ToString()).Count();
                }
            }
        }

        /**
         * Calculates the win ratio given an input of wins and losses.
         */
        public decimal getRatioWithIdAndGameId(int wins, int losses)
        {
            int total = wins + losses;
            // If the user has no games or only draws
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

        /**
         * Returns the win ratio for a given user in the specified game
         * TODO: Potentially remove (no uses)
         */
        public decimal getRatioWithIdAndGameId(string userId, string gameId)
        {
            // get the wins/losses
            // TODO potentially change to use GetAll() in place of null
            int wins = getWinsByIdAndGameId(null, userId, gameId);
            int losses = getLosesByIdAndGameId(null, userId, gameId);
            int total = wins + losses;

            // If the user has played no games or only has draws
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

        /**
         * Returns the win ratio for a given user in the specified game
         * userGameList is used to optimise the algorithm by avoiding excessive calls to GetAll()
         */
        public decimal getRatioWithIdAndGameId(IEnumerable<UserGame> userGameList, string userId, string gameId)
        {
            int wins = getWinsByIdAndGameId(userGameList, userId, gameId);
            int losses = getLosesByIdAndGameId(userGameList, userId, gameId);
            int total = wins + losses;

            // If the user has played no games or only has draws
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

        /**
         * Gets the win ratio for a user in a specified game INCLUDING draws towards the
         * total games.
         */
        public decimal getRatioIncludingDrawWithIdAndGameId(string userId, string gameId)
        {
            int wins = getWinsByIdAndGameId(null, userId, gameId);
            int draws = getDrawsByIdAndGameId(null, userId, gameId);
            int loses = getLosesByIdAndGameId(null, userId, gameId);
            int total = wins + draws + loses;

            // If the user has played no games
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

        /**
         * Returns a list of all user games for a specified game.
         */
        public IEnumerable<UserGame> getUserGamesByGameId(string gameId)
        {
            // If input Id is null or empty all games are returned
            if (gameId==null||gameId== "" || gameId == "0")
            {
                return GetAll();
            }
            else
            {
                return GetAll().Where(userGame => (userGame.GamePlayed.Id.ToString() == gameId));
            }
            
        }
        public IEnumerable<UserGame> getUserGamesByGameIdAndMonth(string gameId, DateTime monthFetched)
        {
            
            // If input Id is null or empty all games are returned
            if (gameId == null || gameId == "" || gameId == "0")
            {
                return _context.UserGames.Include(game => game.GamePlayed).Where(game => game.GamePlayedOn.Month == monthFetched.Month && game.GamePlayedOn.Year == monthFetched.Year);
            }
            else
            {
                IEnumerable<UserGame> x = _context.UserGames.Include(game => game.GamePlayed);
                x = x.Where(game => (game.GamePlayed.Id.ToString() == gameId));
                x = x.Where(game => (game.GamePlayedOn.Month == monthFetched.Month && game.GamePlayedOn.Year == monthFetched.Year));
                return x;
                // below return statement is exactly same as above but it throws error: Method System.String ToString() declared on type System.Int32 cannot be called with instance of type System.Nullable1[System.Int32] string
                //return _context.UserGames.Include(game => game.GamePlayed)
                //    .Where(game => (game.GamePlayed.Id.ToString() == gameId) && game.GamePlayedOn.Month == monthFetched.Month && game.GamePlayedOn.Year == monthFetched.Year);
            }

        }

        /**
         * Returns a list of all user games for a specified user
         */
        public IEnumerable<UserGame> getUserGamesByUserId(string userId)
        {
            return GetAll()
                .Where(userGame => 
                    (userGame.User_01_Id.ToString() == userId) 
                    || userGame.User_02_Id.ToString() == userId);
        }

        /**
         * Returns the number of games a user has played
         */
        public int getTotalGamePlayedByUserId(string userId)
        {
            return GetAll()
                .Where(userGame =>
                    (userGame.User_01_Id.ToString() == userId)
                    || userGame.User_02_Id.ToString() == userId).Count();
        }

        /**
         * Calculates the number of points users gain from a user game
         */
        public int[] CalculatePoints(int flatPoints, 
                                     decimal multiplier, 
                                     int flatLoss, 
                                     decimal lossMultiplier, 
                                     string user1Id, 
                                     string user2Id, 
                                     string winner, 
                                     string gameId)
        {
            // Get the number of games both players have played
            var noOfGamePlayed1 = getTotalGamePlayedByUserId(user1Id);
            var noOfGamePlayed2 = getTotalGamePlayedByUserId(user2Id);

            // Get the number of points both players have before the new game
            var user_01_Points = getUserPoint(user1Id, gameId);
            var user_02_Points = getUserPoint(user2Id, gameId);

            // Initialises the modifier part of the algorithm (the potential difference in point gain based on the
            // difference in points between the two users.
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
                // Equal points, so no modifier is applied
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
                // User 1 has more points so modifier benefits player 2
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
                // User 2 has more points so modifier benefits player 1
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

            // Performs two checks before returning the input
            return CheckForInvalidReduction(CheckForGamesPlayed(calculatedPointsChange, noOfGamePlayed1, noOfGamePlayed2), user_01_Points, user_02_Points);
        }
        /**
         * This method checks that the point changes provided will not reduce either players points below 15 and updates
         * the points change to end with 15 points if it does.
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
            // Same cases as for player 1 repeated
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
         * games are considered for overall not just the individual game the user is playing.
        */
        public int[] CheckForGamesPlayed(int[] points, int gamesPlayed1, int gamesPlayed2)
        {
            // If they are losing points and haven't yet played 5 games
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

        /**
         * Returns the nuumber of points a user has in a specified game
         * This method has been optimised using userSpecificUGList which prevent the need to call GetAll()
         * multiple times.
         */
        public int getUserPoint(IEnumerable<UserGame> userSpecificUGList, string userId, string gameId)
        {
            // If the gameId is null or empty, provides a users overall points.
            if (gameId == null || gameId == "" || gameId == "0")
            {
                var point1 = userSpecificUGList.Where(game => game.User_01_Id == userId).Sum(game => game.User_01_Awarder_Points);
                var point2 = userSpecificUGList.Where(game => game.User_02_Id == userId).Sum(game => game.User_02_Awarder_Points);
                return point1 + point2;
            }
            else
            {
                var point1 = userSpecificUGList.Where(game => game.User_01_Id == userId && game.GamePlayed.Id.ToString() == gameId).Sum(game => game.User_01_Awarder_Points);
                var point2 = userSpecificUGList.Where(game => game.User_02_Id == userId && game.GamePlayed.Id.ToString() == gameId).Sum(game => game.User_02_Awarder_Points);
                return point1 + point2;
            }
        }

        /**
         * Returns the nuumber of points a user has in a specified game
         * TODO: potentially remove this method as optimised version can always be used
         */
        public int getUserPoint(string userId, string gameId)
        {
            // If the gameId is null or empty, provides a users overall points.
            if (gameId == null || gameId == "" || gameId == "0")
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

        /**
         * Returns a user game identified by its Id
         */
        public UserGame GetById(int userGameId)
        {
            return GetAll().FirstOrDefault(userGame => userGame.Id == userGameId);
        }

        /**
         * Adds a user game to the database
         */
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

        /**
         * Delete a user game from the database
         * TODO: implemenet and call updatesubsequentgames method to repair database points after deletion
         */
        public async Task DeleteUserGame(int userGameId)
        {
            var userGame = GetById(userGameId);

            _context.Remove(userGame);
            await _context.SaveChangesAsync(); // commits changes to DB.
        }

        /**
         * Edit a user game in the database
         * TODO: implement and call updatesubsequentgames method to repair database points after editing
         */
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

        /**
         * Sets the Image of a user game
         * TODO: remove this? we never display images for a user game
         */
        public Task SetGameImageAsync(int userGameId, Uri uri)
        {
            throw new NotImplementedException();
        }

        /**
         * Counts the number of wins a given user has
         * TODO; remove this method, never used and a better alternate is provided.
         */
        public int CountWinsForUser(string userId)
        {
            return GetAll().Where(uGame => uGame.Winner == userId).Count();
        }

    }
}
