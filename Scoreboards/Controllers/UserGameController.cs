using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Scoreboards.Data;
using Scoreboards.Data.Models;
using Scoreboards.Hubs;
using Scoreboards.Models.UserGames;
using System;
using System.Threading.Tasks;

namespace Scoreboards.Controllers
{
    public class UserGameController : Controller
    {
        private readonly IUserGame _userGameService;
        private readonly IApplicationUser _userService;
        private readonly IGame _gameService;
        private readonly UserManager<ApplicationUser> _userManager;
        // SignalR
        private readonly IHubContext<ScoreboardsHub> _hubContext;

        public UserGameController(
              IUserGame userGameService
            , IGame gameService
            , IApplicationUser userService
            , UserManager<ApplicationUser> userManager
            , IHubContext<ScoreboardsHub> hubContext)
        {
            _gameService = gameService;
            _userGameService = userGameService;
            _userManager = userManager;
            _userService = userService;
            _hubContext = hubContext;
        }

        public IActionResult Index(int userGameId)
        {
            // When new game is created it is redirected to index page not Home
            // 
            // TODO: Prepopulate form with data about games
            // Palyers names
            // Create view model to feed to the page
            var userGame = _userGameService.GetById(userGameId);
            var user_01 = _userService.GetById(userGame.User_01_Id);
            var user_02 = _userService.GetById(userGame.User_02_Id);

            var model = new UserGameListingModel
            {
                Id = userGame.Id,
                //Game played Date
                GamePlayedOn = userGame.GamePlayedOn,

                //Players detail
                User_01 = user_01,
                User_01_Team = userGame.User_01_Team,
                User_02 = user_02,
                User_02_Team = userGame.User_02_Team,

                // Game Name
                GameName = userGame.GamePlayed.GameName,

                //Score 
                GameScore = userGame.GameScoreUser01 + " : " + userGame.GameScoreUser02,

                //Winner, “USER_01_Id”, “USER_02_Id”, “DRAW”
                Winner = userGame.Winner,
            };

            return View(model);
        }

        /**
         * Creates new Post in a Forum determined by forumId
         * It prepopulates the form with user and Forum data
         */
        public IActionResult CreateNewUserGame()
        {
            var games = _gameService.GetAll();

            var users = _userService.GetAll();

            var model = new NewUserGameModel
            {
                GamesList = games,
                UsersList = users
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> AddUserGame(NewUserGameModel model)
        {
            var userId = _userManager.GetUserId(User);
            //_userManager is a built in service. From
            // Microsoft.AspNetCore.Identity; - Provides API to interact with Users in
            // Data store.
            //User is a built in Object that contains Current User info.
            // We may Use current user later as a refery
            var user = await _userManager.FindByIdAsync(userId);


            var userGame = BuildUserGame(model);
            //TODO: User management rating.

            await _userGameService.AddUserGameAsync(userGame);

            // SignalR send message to All that DB was updated
            await _hubContext.Clients.All.SendAsync("Notify", $"Created new UserGame at : {DateTime.Now}");

            return RedirectToAction("Index", "Home");
        }

        // =====================
        private int CalculatePointsOld(int flatPoints, decimal multiplier, string userId, string opponentId, string winner)
        {
            var noOfGamePlayed = _userGameService.getTotalGamePlayedByUserId(userId);
            var user_01_Points = _userGameService.getUserPoint(userId);
            var user_02_Points = _userGameService.getUserPoint(opponentId);
            if (winner.ToLower() == "draw")
            {//in case the game is "draw"
                if (user_01_Points >= user_02_Points)
                {
                    return 0;
                }
                else
                {
                    return (int)Math.Round((decimal)(user_02_Points - user_01_Points) / (user_02_Points + user_01_Points) * multiplier, 2);
                }
            }
            else
            {// in case game is not "draw"
                if (winner == userId)
                {// user1 win
                    if (user_01_Points == user_02_Points)
                    {
                        return flatPoints;
                    }
                    else if (user_01_Points > user_02_Points)
                    {
                        if (user_01_Points + user_02_Points <= 100)
                        {
                            return (int)(flatPoints - Math.Round((user_01_Points - user_02_Points) / (decimal)100.0 * multiplier));
                        }
                        else
                        {
                            return (int)(flatPoints - Math.Round((user_01_Points - user_02_Points) / (user_01_Points + user_02_Points) * multiplier));
                        }
                    }
                    else
                    {
                        if (user_01_Points + user_02_Points <= 100)
                        {
                            return (int)(flatPoints + Math.Round((user_02_Points - user_01_Points) / (decimal)(100.0) * multiplier));
                        }
                        else
                        {
                            return (int)(flatPoints + Math.Round((user_02_Points - user_01_Points) / (user_02_Points + user_01_Points) * multiplier));
                        }
                    }
                }
                else
                {// user 2 win
                    if (noOfGamePlayed < 5)
                    {// played less than 5
                        return 0;
                    }
                    else
                    {// played more than 5
                        if (user_01_Points == user_02_Points)
                        {
                            return -flatPoints;
                        }
                        else if (user_01_Points > user_02_Points)
                        {
                            if (user_01_Points + user_02_Points <= 100)
                            {
                                return (int)(-flatPoints - Math.Round((user_01_Points - user_02_Points) / (decimal)(100.0) * multiplier));
                            }
                            else
                            {
                                return (int)(-flatPoints - Math.Round((user_01_Points - user_02_Points) / (user_01_Points + user_02_Points) * multiplier));
                            }
                        }
                        else
                        {
                            if (user_01_Points + user_02_Points <= 100)
                            {
                                return (int)(-flatPoints + Math.Round((user_02_Points - user_01_Points) / (decimal)(100.0) * multiplier));
                            }
                            else
                            {
                                return (int)(-flatPoints + Math.Round((user_02_Points - user_01_Points) / (user_02_Points + user_01_Points) * multiplier));
                            }
                        }
                    }

                }
            }
        }
        private int[] Whatever(int flatPoints, decimal multiplier, string userId, string opponentId, string winner)
        {
            return new int[2] { CalculatePointsOld(flatPoints, multiplier, userId, opponentId, winner), CalculatePointsOld(flatPoints, multiplier, opponentId, userId, winner) };
        }
        private UserGame BuildUserGame(NewUserGameModel model)
        {
            var user1 = _userService.GetById(model.User_01_Id);
            var user2 = _userService.GetById(model.User_02_Id);
            var gamePlayed = _gameService.GetByName(model.GamePlayedName);

            // Check the winner
            var winner = "DRAW";

            //var scores = model.GameScore.Split(":");
            var player1Score = Convert.ToInt32(model.GameScoreUser01);
            var player2Score = Convert.ToInt32(model.GameScoreUser02);

            //if (scores.Length != 0)
            //{
            //    player1Score = Convert.ToInt32(scores.GetValue(0));
            //    player2Score = Convert.ToInt32(scores.GetValue(1));
            //}


            if (player1Score > player2Score)
            {// user1 won
                winner = user1.Id;
            }

            if (player1Score < player2Score)
            {// user2 won
                winner = user2.Id;
            }

            // set flat points & multiplier that will be used to calculate the point that each user will get
            var flatPoints = 15;
            var multiplier = (decimal)10.0;

            // CalculatePoints method passes 5 parameters
            // 1. flat points
            // 2. multiplier
            // 3. user id that you want to calculate
            // 4. opponent user id
            // 5. winner id of current game
            //int [] pointsCalculated = Whatever(flatPoints, multiplier, user1.Id, user2.Id, winner);
            int[] pointsCalculated = _userGameService.CalculatePoints(flatPoints, multiplier, user1.Id, user2.Id, winner);
            return new UserGame
            {
                User_01_Id = model.User_01_Id,
                User_01_Team = model.User_01_Team,
                User_02_Id = model.User_02_Id,
                User_02_Team = model.User_02_Team,

                //Score 
                GameScoreUser01 = player1Score,
                GameScoreUser02 = player2Score,

                //Winner, “USER_01_Id”, “USER_02_Id”, “DRAW”
                Winner = winner,

                //Referee details. Only keep their User.Id
                RefereeUserId = model.RefereeUserId,

                // Name of the game
                GamePlayed = gamePlayed,

                // User_01_Awarder_Points
                User_01_Awarder_Points = pointsCalculated[0],
                // User_02_Awarder_Points
                User_02_Awarder_Points = pointsCalculated[1]

            };
        }
    }
}
