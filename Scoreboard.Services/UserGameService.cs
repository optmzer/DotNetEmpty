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
        private readonly IApplicationUser _userService;
        private readonly IMonthlyWinners _monthlyWinnersService;
        private readonly int _flatPointsGain = 15;
        private readonly decimal _gainMultiplier = 10;
        private readonly int _flatPointsLoss = 7;
        private readonly decimal _lossMultiplier = 7;
        public UserGameService(ApplicationDbContext context, IApplicationUser userService, IMonthlyWinners monthlyWinnersService)
        {
            _context = context;
            _userService = userService;
            _monthlyWinnersService = monthlyWinnersService;
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
        
        public int getWinsByIdAndGameId(IEnumerable<UserGame> preparedData, string userId, string gameId)
        {
            // if preparedData is provided, no need to GetAll
            if (preparedData == null)
            {
                // if gameName is null, provide overall
                // else provide overall
                if (gameId == null || gameId == "")
                {
                    return GetAll().Where(userGame => (userGame.User_01_Id == userId || userGame.User_02_Id == userId) && userGame.Winner == userId).Count();
                }
                else
                {
                    return GetAll().Where(userGame => ((userGame.User_01_Id == userId || userGame.User_02_Id == userId) && userGame.Winner == userId) && gameId == userGame.GamePlayed.Id.ToString()).Count();
                }
            } else
            {
                // if gameName is null, provide overall
                // else provide overall
                if (gameId == null || gameId == "")
                {
                    return preparedData.Where(userGame => (userGame.User_01_Id == userId || userGame.User_02_Id == userId) && userGame.Winner == userId).Count();
                }
                else
                {
                    return preparedData.Where(userGame => ((userGame.User_01_Id == userId || userGame.User_02_Id == userId) && userGame.Winner == userId) && gameId == userGame.GamePlayed.Id.ToString()).Count();
                }
            }
        }
        public int getDrawsByIdAndGameId(IEnumerable<UserGame> preparedData, string userId, string gameId)
        {
            if (preparedData == null)
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
            } else
            {
                // if gameName is null, provide overall
                // else provide overall
                if (gameId == null || gameId == "")
                {
                    return preparedData.Where(userGame => (userGame.User_01_Id == userId || userGame.User_02_Id == userId) && userGame.Winner.ToLower() == "draw").Count();
                }
                else
                {
                    return preparedData.Where(userGame => ((userGame.User_01_Id == userId || userGame.User_02_Id == userId) && userGame.Winner.ToLower() == "draw") && gameId == userGame.GamePlayed.Id.ToString()).Count();
                }
            }
        }
        public int getLosesByIdAndGameId(IEnumerable<UserGame> preparedData, string userId, string gameId)
        {
            if (preparedData == null)
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
            } else
            {
                // if gameName is null, provide overall
                // else provide overall
                if (gameId == null || gameId == "")
                {
                    return preparedData.Where(userGame => (userGame.User_01_Id == userId || userGame.User_02_Id == userId) && userGame.Winner != userId && userGame.Winner.ToLower() != "draw").Count();
                }
                else
                {
                    return preparedData.Where(userGame => ((userGame.User_01_Id == userId || userGame.User_02_Id == userId) && userGame.Winner != userId && userGame.Winner.ToLower() != "draw") && gameId == userGame.GamePlayed.Id.ToString()).Count();
                }
            }
        }
        public decimal getRatioWithIdAndGameId(int wins, int losses)
        {
            int total = wins + losses;
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
        public decimal getRatioWithIdAndGameId(string userId, string gameId)
        {
            int wins = getWinsByIdAndGameId(null, userId, gameId);
            int losses = getLosesByIdAndGameId(null, userId, gameId);
            int total = wins + losses;
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
        public decimal getRatioWithIdAndGameId(IEnumerable<UserGame> userGameList, string userId, string gameId)
        {
            int wins = getWinsByIdAndGameId(userGameList, userId, gameId);
            int losses = getLosesByIdAndGameId(userGameList, userId, gameId);
            int total = wins + losses;
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
            int wins = getWinsByIdAndGameId(null, userId, gameId);
            int draws = getDrawsByIdAndGameId(null, userId, gameId);
            int loses = getLosesByIdAndGameId(null, userId, gameId);
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

        /**
         * 
         */
        public int[] CalculatePoints(int user_01_Points, int user_02_Points, string winner, string gameId, int user1GamesPlayed, int user2GamesPlayed)
        {

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

            return CheckForInvalidReduction(CheckForGamesPlayed(calculatedPointsChange, user1GamesPlayed, user2GamesPlayed), user_01_Points, user_02_Points);
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
         * Checks the users have both played more than 5 game and if they haven't returns updated points change
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
        public int getUserPoint(IEnumerable<UserGame> userSpecificUGList, string userId, string gameId)
        {
            if (gameId == null || gameId == "")
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
            await UpdateSubsequentGames(userGame, "Delete");
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
            await UpdateSubsequentGames(game, "Edit");
        }

        public async Task UpdateSubsequentGames(UserGame baseGame, string identifier)
        {
            Dictionary<string, int[]> usersPoints = new Dictionary<string, int[]>();
            // Check if the game is edited or deleted
            // If edited find the score for the players up to and including that game
            // If deleted find the score for the players up to and excluding the game
            if (identifier == "Delete")
            {
                // Get user ones points up to and excluding the deleted game
                usersPoints.Add(baseGame.User_01_Id, GetUserGamesAndPointsUpToGame(baseGame, baseGame.User_01_Id));

                // Get user twos points up to and excluding the deleted game
                usersPoints.Add(baseGame.User_02_Id, GetUserGamesAndPointsUpToGame(baseGame, baseGame.User_02_Id));
            }
            else if (identifier == "Edit")
            {
                // Get user ones points up to and including the edited game
                int[] UserOneGamesAndPoints = GetUserGamesAndPointsUpToGame(baseGame, baseGame.User_01_Id);
                UserOneGamesAndPoints[1] += baseGame.User_01_Awarder_Points;
                usersPoints.Add(baseGame.User_01_Id, UserOneGamesAndPoints);

                // Get user twos points up to and including the edited game
                int[] UserTwoGamesAndPoints = GetUserGamesAndPointsUpToGame(baseGame, baseGame.User_02_Id);
                UserTwoGamesAndPoints[1] += baseGame.User_02_Awarder_Points;
                usersPoints.Add(baseGame.User_02_Id, UserTwoGamesAndPoints);
            }
            // Create a list of all games either user is a part of (sorted by Id)
            SortedList<int, UserGame> userGames = new SortedList<int, UserGame>();

            // Get a list of all user ones games and add it to the list
            IEnumerable<UserGame> userOneGames = getUserGamesByUserId(baseGame.User_01_Id).Where(game => 
                game.GamePlayed == baseGame.GamePlayed && game.Id > baseGame.Id);
            foreach (UserGame game in userOneGames)
            {
                userGames.Add(game.Id, game);
            }

            // Get a list of all user twos games and add it too the list (avoiding duplicates)
            IEnumerable<UserGame> userTwoGames = getUserGamesByUserId(baseGame.User_02_Id).Where(game =>
                game.GamePlayed == baseGame.GamePlayed && game.Id > baseGame.Id);
            foreach (UserGame game in userTwoGames)
            {
                if (!userGames.Keys.Contains(game.Id))
                {
                    userGames.Add(game.Id, game);
                }
            }
       
            // Take the first (oldest) game in the queue, Check if a new player is involved
            // Loop through the games using while !queue.isEmpty() and remove each game when you're
            // done with it.
            while (!userGames.Any())
            {
                UserGame currentUserGame = userGames.First().Value;
                // If a new user is involved add their games to the priority queue
                if (!usersPoints.Keys.Contains(currentUserGame.User_01_Id))
                {
                    usersPoints.Add(currentUserGame.User_01_Id, GetUserGamesAndPointsUpToGame(currentUserGame, currentUserGame.User_01_Id));
                    IEnumerable<UserGame> addedUsersGames = getUserGamesByUserId(currentUserGame.User_01_Id).Where(game =>
                        game.GamePlayed == baseGame.GamePlayed && game.Id > currentUserGame.Id);
                    foreach (UserGame game in addedUsersGames)
                    {
                        if (!userGames.Keys.Contains(game.Id))
                        {
                            userGames.Add(game.Id, game);
                        }
                    }
                }
                else if (!usersPoints.Keys.Contains(currentUserGame.User_02_Id))
                {
                    usersPoints.Add(currentUserGame.User_02_Id, GetUserGamesAndPointsUpToGame(currentUserGame, currentUserGame.User_02_Id));
                    IEnumerable<UserGame> addedUsersGames = getUserGamesByUserId(currentUserGame.User_02_Id).Where(game =>
                        game.GamePlayed == baseGame.GamePlayed && game.Id > currentUserGame.Id);
                    foreach (UserGame game in addedUsersGames)
                    {
                        if (!userGames.Keys.Contains(game.Id))
                        {
                            userGames.Add(game.Id, game);
                        }
                    }
                }
                string winner;
                if (currentUserGame.Winner == currentUserGame.User_01_Id)
                {
                    winner = "user1";
                }
                else
                {
                    winner = "user2";
                }

                CalculatePoints(usersPoints[currentUserGame.User_01_Id][1], 
                                usersPoints[currentUserGame.User_02_Id][1],
                                winner, 
                                currentUserGame.GamePlayed.Id.ToString(),
                                usersPoints[currentUserGame.User_01_Id][0],
                                usersPoints[currentUserGame.User_02_Id][0]);
            }

            // Update the game and the local score of the two players

            // Repeat for the next game in the priority queue till it's empty

            // Save the changes (and check if any new games have been submitted based on Id)

            // If a new game has been submitted update that.

            // Repeat checking until no new game added.
        }

        /**
         * This method is used to help recalculate the points distribution after a game is edited or deleted.
         * It is used when a user is first included in the process and calculates their total points up to the 
         * selected game. It also returns the number of games a user has played
        */
        public int[] GetUserGamesAndPointsUpToGame(UserGame userGame, string userId )
        {
            var userOneGames = _context.UserGames.Where(game => game.User_01_Id == userId
                    && game.GamePlayed.Id == userGame.GamePlayed.Id &&
                    game.Id < userGame.Id);
            var userTwoGames = _context.UserGames.Where(game => game.User_02_Id == userId
                && game.GamePlayed.Id == userGame.GamePlayed.Id &&
                game.Id < userGame.Id);

            return new int[2]{userOneGames.Count() + userTwoGames.Count(),
                userOneGames.Sum(game => game.User_01_Awarder_Points) + userTwoGames.Sum(game => game.User_02_Awarder_Points) };
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
