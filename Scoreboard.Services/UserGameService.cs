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
        private readonly int _flatPointsGain = 15;
        private readonly decimal _gainMultiplier = 10;
        private readonly int _flatPointsLoss = 7;
        private readonly decimal _lossMultiplier = 7;

        public UserGameService(ApplicationDbContext context, IApplicationUser userService)
        {
            _context = context;
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


        public int[] CalculatePoints(int user_01_Points, int user_02_Points, string winner, string gameId)
        {
            // Initialises the modifier part of the algorithm (the potential difference in point gain based on the
            // difference in points between the two users.
            var multiplierModified = (decimal)0;
            var modifiedLoss = (decimal)0;
            if (user_01_Points + user_02_Points <= 100)
            {
                multiplierModified = Math.Round(Math.Abs(user_01_Points - user_02_Points) / (decimal)100.0 * _gainMultiplier);
                modifiedLoss = Math.Round(Math.Abs(user_01_Points - user_02_Points) / (decimal)100.0 * _lossMultiplier);
            }
            else
            {
                multiplierModified = Math.Round((Math.Abs(user_02_Points - user_01_Points) / (decimal)(user_02_Points + user_01_Points)) * _gainMultiplier);
                modifiedLoss = Math.Round((Math.Abs(user_02_Points - user_01_Points) / (decimal)(user_02_Points + user_01_Points)) * _lossMultiplier);

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
                    if (winner == "user1")
                    {
                        calculatedPointsChange = new int[2] { _flatPointsGain, -_flatPointsLoss };
                    }
                    else
                    {
                        calculatedPointsChange = new int[2] { -_flatPointsLoss, _flatPointsGain };
                    }
                }
                // User 1 has more points so modifier benefits player 2
                else if (user_01_Points > user_02_Points)
                {
                    if (winner == "user1")
                    {
                        calculatedPointsChange = new int[2] { (int)(_flatPointsGain - multiplierModified), (int)(-_flatPointsLoss + modifiedLoss) };
                    }
                    else
                    {
                        calculatedPointsChange =  new int[2] { (int)(-_flatPointsLoss - modifiedLoss), (int)(_flatPointsGain + multiplierModified) };
                    }
                }
                // User 2 has more points so modifier benefits player 1
                else
                {
                    if (winner == "user1")
                    {
                        calculatedPointsChange =  new int[2] { (int)(_flatPointsGain + multiplierModified), (int)(-_flatPointsLoss - modifiedLoss) };
                    }
                    else
                    {
                        calculatedPointsChange =  new int[2] { (int)(-_flatPointsLoss + modifiedLoss), (int)(_flatPointsGain - multiplierModified) };
                    }
                }
            }

            return CheckForInvalidReduction(calculatedPointsChange, user_01_Points, user_02_Points);
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
         * Returns the nuumber of points a user has in a specified game
         * This method has been optimised using userSpecificUGList which prevent the need to call GetAll()
         * multiple times.
         */
        public int getUserPoint(IEnumerable<UserGame> userSpecificUGList, string userId, string gameId)
        {
            // If the gameId is null or empty, provides a users overall points.
            if (gameId == null || gameId == "" || gameId == "0")
            {
                var point1 = userSpecificUGList.Where(game => 
                                                      game.User_01_Id == userId)
                                                      .Sum(game => game.User_01_Awarder_Points);
                var point2 = userSpecificUGList.Where(game => 
                                                      game.User_02_Id == userId)
                                                      .Sum(game => game.User_02_Awarder_Points);
                return point1 + point2;
            }
            else
            {
                var point1 = userSpecificUGList.Where(game => 
                                                      game.User_01_Id == userId && game.GamePlayed.Id.ToString() == gameId)
                                                      .Sum(game => game.User_01_Awarder_Points);
                var point2 = userSpecificUGList.Where(game => 
                                                      game.User_02_Id == userId && game.GamePlayed.Id.ToString() == gameId)
                                                      .Sum(game => game.User_02_Awarder_Points);
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
                var point1 = listOfUserGames.Where(game => 
                                                   game.User_01_Id == userId)
                                                   .Sum(game => game.User_01_Awarder_Points);
                var point2 = listOfUserGames.Where(game => 
                                                   game.User_02_Id == userId)
                                                   .Sum(game => game.User_02_Awarder_Points);
                return point1 + point2;
            }
            else
            {
                var listOfUserGames = getUserGamesByUserId(userId);
                var point1 = listOfUserGames.Where(game => 
                                                   game.User_01_Id == userId && game.GamePlayed.Id.ToString() == gameId)
                                                   .Sum(game => game.User_01_Awarder_Points);
                var point2 = listOfUserGames.Where(game => 
                                                   game.User_02_Id == userId && game.GamePlayed.Id.ToString() == gameId)
                                                   .Sum(game => game.User_02_Awarder_Points);
                return point1 + point2;
            }
        }

        public int getUserPointsByMonth(string userId, string gameId)
        {
            // If the gameId is null or empty, provides a users overall points.
            if (gameId == null || gameId == "")
            {
                var listOfUserGames = getUserGamesByUserId(userId);
                var point1 = listOfUserGames.Where(game =>
                                                   game.User_01_Id == userId 
                                                   && game.GamePlayedOn.Month == DateTime.Now.Month)
                                                   .Sum(game => game.User_01_Awarder_Points);
                var point2 = listOfUserGames.Where(game =>
                                                   game.User_02_Id == userId
                                                   && game.GamePlayedOn.Month == DateTime.Now.Month)
                                                   .Sum(game => game.User_02_Awarder_Points);
                return point1 + point2;
            }
            else
            {
                var listOfUserGames = getUserGamesByUserId(userId);
                var point1 = listOfUserGames.Where(game =>
                                                   game.User_01_Id == userId 
                                                   && game.GamePlayed.Id.ToString() == gameId
                                                   && game.GamePlayedOn.Month == DateTime.Now.Month)
                                                   .Sum(game => game.User_01_Awarder_Points);
                var point2 = listOfUserGames.Where(game =>
                                                   game.User_02_Id == userId 
                                                   && game.GamePlayed.Id.ToString() == gameId
                                                   && game.GamePlayedOn.Month == DateTime.Now.Month)
                                                   .Sum(game => game.User_02_Awarder_Points);
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
         * Deletes all user games for a specified game, used when the game is being deleted
         */
        public async Task DeleteUserGamesForGame(Game game)
        {
            var userGames = getUserGamesByGameId(game.Id.ToString());
            foreach (var userGame in userGames)
            {

                _context.Remove(userGame);
            }

            await _context.SaveChangesAsync();
        }

        /**
         * Delete a user game from the database
         */
        public async Task DeleteUserGame(int userGameId)
        {
            var userGame = GetById(userGameId);
            await UpdateSubsequentGames(userGame, "Delete");
            _context.Remove(userGame);
            await _context.SaveChangesAsync(); // commits changes to DB.
        }

        /**
         * Deletes UserGames based on date supplied
         */ 
        public async Task DeleteUserGameByMonth(DateTime month)
        {
            IEnumerable<UserGame> list_to_delete = _context.UserGames
                .Where(game => game.GamePlayedOn.Month == month.Month);

            //uncomment for production
            _context.RemoveRange(list_to_delete);

            await _context.SaveChangesAsync();
        }

        /**
         * Performs Hard Reset
         * Drops all entires in the UserGames table
         * There is no return after this point.
         */ 
        public async Task DeleteAllUserGames()
        {
            await _context.Database.ExecuteSqlCommandAsync("TRUNCATE TABLE UserGames;");
            await _context.SaveChangesAsync();
        }

        /**
         * Edit a user game in the database
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
            await UpdateSubsequentGames(game, "Edit");
        }

        /**
         * This method is used to repair the database when a game is deleted or edited
         * It works by finding the users points up to the edited/deleted game then
         * manually recalculating all the point changes.
         */
        public async Task UpdateSubsequentGames(UserGame baseGame, string identifier)
        {
            // In the dictionary the key is the users Id, the first value in the int array
            // is the games played the second is the users points calculated to the
            // current game
            Dictionary<string, int> usersPoints = new Dictionary<string, int>();
            // Check if the game is edited or deleted
            // If edited find the score for the players up to and including that game
            // If deleted find the score for the players up to and excluding the game
            if (identifier == "Delete")
            {
                // Get user ones points up to and excluding the deleted game
                usersPoints.Add(baseGame.User_01_Id, GetUserPointsUpToGame(baseGame, baseGame.User_01_Id));

                // Get user twos points up to and excluding the deleted game
                usersPoints.Add(baseGame.User_02_Id, GetUserPointsUpToGame(baseGame, baseGame.User_02_Id));
            }
            else if (identifier == "Edit")
            {
                // Get user ones points up to and including the edited game
                int UserOnePoints = GetUserPointsUpToGame(baseGame, baseGame.User_01_Id);

                // Get user twos points up to and including the edited game
                int UserTwoPoints = GetUserPointsUpToGame(baseGame, baseGame.User_02_Id);

                string winner = "DRAW";

                if (baseGame.Winner == baseGame.User_01_Id)
                {
                    winner = "user1";
                }
                else if (baseGame.Winner == baseGame.User_02_Id)
                {
                    winner = "user2";
                }

                int[] newPoints;
                newPoints = CalculatePoints(UserOnePoints,
                                UserTwoPoints,
                                winner,
                                baseGame.GamePlayed.Id.ToString());

                _context.Entry(baseGame).State = EntityState.Modified;
                baseGame.User_01_Awarder_Points = newPoints[0];
                baseGame.User_02_Awarder_Points = newPoints[1];
                await _context.SaveChangesAsync();

                UserOnePoints += baseGame.User_01_Awarder_Points;
                usersPoints.Add(baseGame.User_01_Id, UserOnePoints);

                UserTwoPoints += baseGame.User_02_Awarder_Points;
                usersPoints.Add(baseGame.User_02_Id, UserTwoPoints);
            }
            // Create a list of all games either user is a part of (sorted by Id)
            SortedList<int, UserGame> userGames = new SortedList<int, UserGame>();

            // Get a list of all user ones games and add it to the list
            IEnumerable<UserGame> userOneGames = getUserGamesByUserId(baseGame.User_01_Id)
                                                 .Where(game => game.GamePlayed == baseGame.GamePlayed 
                                                 && game.Id > baseGame.Id);
            foreach (UserGame game in userOneGames)
            {
                userGames.Add(game.Id, game);
            }

            // Get a list of all user twos games and add it too the list (avoiding duplicates)
            IEnumerable<UserGame> userTwoGames = getUserGamesByUserId(baseGame.User_02_Id)
                                                 .Where(game => game.GamePlayed == baseGame.GamePlayed 
                                                 && game.Id > baseGame.Id);
            foreach (UserGame game in userTwoGames)
            {
                // Checks the game isn't already in the list before adding it. (Checks for other games
                // where the two users have played together).
                if (!userGames.Keys.Contains(game.Id))
                {
                    userGames.Add(game.Id, game);
                }
            }
            int latestId = baseGame.Id;


            // Loop through the games starting with the oldest game in the queue
            // If a new player is involved, calculate his points and games played then add his
            // games to the queue.
            // Calculate the change in points for both players then update their local values.
            // Remove the game from the queue and loop to the next value.
            while (userGames.Any())
            {
                // Get the next value in the queue
                UserGame currentUserGame = userGames.First().Value;
                string userOneId = currentUserGame.User_01_Id;
                string userTwoId = currentUserGame.User_02_Id;
                // If a new user is involved add their games to the priority queue
                if (!usersPoints.Keys.Contains(userOneId))
                {
                    usersPoints.Add(userOneId,
                                    GetUserPointsUpToGame(currentUserGame, userOneId));
                    IEnumerable<UserGame> addedUsersGames = getUserGamesByUserId(userOneId)
                                                            .Where(game => game.GamePlayed == baseGame.GamePlayed
                                                            && game.Id > currentUserGame.Id);
                    foreach (UserGame game in addedUsersGames)
                    {
                        // Checks the game isn't already part of the queue
                        if (!userGames.Keys.Contains(game.Id))
                        {
                            userGames.Add(game.Id, game);
                        }
                    }
                }
                // Repeating previous statement with user 2.
                else if (!usersPoints.Keys.Contains(userTwoId))
                {
                    usersPoints.Add(userTwoId,
                                    GetUserPointsUpToGame(currentUserGame, userTwoId));
                    IEnumerable<UserGame> addedUsersGames = getUserGamesByUserId(userTwoId)
                                                            .Where(game => game.GamePlayed == baseGame.GamePlayed
                                                            && game.Id > currentUserGame.Id);
                    foreach (UserGame game in addedUsersGames)
                    {
                        if (!userGames.Keys.Contains(game.Id))
                        {
                            userGames.Add(game.Id, game);
                        }
                    }
                }

                // Calculates the winner and sets the strings to their appropriate values.
                string winner = "DRAW";
                if (currentUserGame.Winner == userOneId)
                {
                    winner = "user1";
                }
                else if (currentUserGame.Winner == userTwoId)
                {
                    winner = "user2";
                }

                // Calculates the change in points for the users based on edited/deleted game
                // changes
                int[] newPoints;
                newPoints = CalculatePoints(usersPoints[userOneId],
                                usersPoints[userTwoId],
                                winner,
                                currentUserGame.GamePlayed.Id.ToString());

                // Adds the change to user ones local total points
                usersPoints[userOneId] += newPoints[0];
                // Adds the change to user twos local total points
                usersPoints[userTwoId] += newPoints[1];

                // Update and save the new points changes to the game.
                _context.Entry(currentUserGame).State = EntityState.Modified;
                currentUserGame.User_01_Awarder_Points = newPoints[0];
                currentUserGame.User_02_Awarder_Points = newPoints[1];
                await _context.SaveChangesAsync();

                userGames.Remove(currentUserGame.Id);
                latestId = currentUserGame.Id;
            }

            // Check if any new games have been added after the database is done.
            //Boolean hasNewGames = true;
            //
            //while (hasNewGames)
            //{
            //    var listOfUserGames = _context.UserGames.Where(game =>
            //                                                   game.Id > latestId && (
            //                                                   usersPoints.Keys.Contains(game.User_01_Id) ||
            //                                                   usersPoints.Keys.Contains(game.User_02_Id) ||
            //                                                   game.GamePlayed == baseGame.GamePlayed
            //                                                   ));
            //    if (!listOfUserGames.Any())
            //    {
            //        hasNewGames = false;
            //    }
            //    else
            //    {
            //        // TODO sort out any games added to the database while the database is updated after editing/deleting
            //        // Placeholder
            //        hasNewGames = false;
            //    }
            //}
        }

        /**
         * This method is used to help recalculate the points distribution after a game is edited or deleted.
         * It is used when a user is first included in the process and calculates their total points up to the 
         * selected game. It also returns the number of games a user has played
         * 
         * It only factors in games for the current Month.
        */
        public int GetUserPointsUpToGame(UserGame userGame, string userId )
        {
            var userPointsOne = _context.UserGames.Where(game => game.User_01_Id == userId
                               && game.GamePlayed.Id == userGame.GamePlayed.Id 
                               && game.Id < userGame.Id
                               && game.GamePlayedOn.Month == DateTime.Now.Month)
                               .Sum(game => game.User_01_Awarder_Points);
            var userPointsTwo = _context.UserGames.Where(game => game.User_02_Id == userId
                               && game.GamePlayed.Id == userGame.GamePlayed.Id
                               && game.Id < userGame.Id
                               && game.GamePlayedOn.Month == DateTime.Now.Month)
                               .Sum(game => game.User_02_Awarder_Points);

            return userPointsOne + userPointsTwo;
        }

        /**
         * Counts the number of wins a given user has
         * TODO; remove this method, never used and a better alternate is provided.
         */
        public int CountWinsForUser(string userId)
        {
            return GetAll().Where(uGame => uGame.Winner == userId).Count();
        }

        public string GetLastMonthWinner(string gameId)
        {
            var time = DateTime.Now.AddMonths(-1);
            if (gameId == null || gameId == "" || gameId.ToLower() == "overall")
            {
                gameId = "";
            }
            IEnumerable<UserGame> listOfGames = getUserGamesByGameIdAndMonth(gameId, time);
            // userIdList contains userId as key and point gained as value
            Dictionary<string, int> userIdDictionary = new Dictionary<string, int>();
            // prepare userIdDictionary that played game last month (if game id is specified only those who played that game last month)
            foreach (UserGame ug in listOfGames)
            {
                string user1_Id = ug.User_01_Id;
                string user2_Id = ug.User_02_Id;
                if (!userIdDictionary.Keys.Any(user1_Id.Contains))
                {
                    userIdDictionary.Add(user1_Id, ug.User_01_Awarder_Points);
                }
                else
                {
                    userIdDictionary[user1_Id] += ug.User_01_Awarder_Points;
                }
                if (!userIdDictionary.Keys.Any(user2_Id.Contains))
                {
                    userIdDictionary.Add(user2_Id, ug.User_02_Awarder_Points);
                }
                else
                {
                    userIdDictionary[user2_Id] += ug.User_02_Awarder_Points;
                }
            }
           return userIdDictionary.Aggregate((x, y) => x.Value > y.Value ? x : y).Key;

        }

      
    }
}
