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

        public IActionResult EditUserGame(int userGameId)
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
                User_01_Id = user_01.Id,
                User_01_Name = user_01.UserName,
                User_01_Team = userGame.User_01_Team,

                User_02_Id = user_02.Id,
                User_02_Name = user_02.UserName,
                User_02_Team = userGame.User_02_Team,

                // Game Name
                GameName = userGame.GamePlayed.GameName,

                //Score 
                GameScoreUser01 = userGame.GameScoreUser01,
                GameScoreUser02 = userGame.GameScoreUser02,

                //Winner, “USER_01_Id”, “USER_02_Id”, “DRAW”
                Winner = userGame.Winner,
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditUserGame(UserGameListingModel model)
        {
            var newUserGameContent = new NewUserGameModel
            {
                GamePlayedName = model.GameName,

                //Players detail
                User_01_Id = model.User_01_Id,
                User_01_Team = model.User_01_Team,

                User_02_Id = model.User_02_Id,
                User_02_Team = model.User_02_Team,

                //Score 
                GameScoreUser01 = model.GameScoreUser01,
                GameScoreUser02 = model.GameScoreUser02,
            };

            var userGame = BuildUserGame(newUserGameContent);
            userGame.Id = model.Id;

            await _userGameService.EditUserGame(userGame);

            // SignalR send message to All that DB was updated
            await _hubContext.Clients.All.SendAsync("Notify", $"Created new UserGame at : {DateTime.Now}");

            return RedirectToAction("Index", "Home");
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

        public IActionResult DeleteUserGame(int userGameId)
        {
            // Get game and Ask user if they sure they want to delete it
            var game = _userGameService.GetById(userGameId);
            var user01 = _userService.GetById(game.User_01_Id);
            var user02 = _userService.GetById(game.User_02_Id);

            var model = new UserGameListingModel
            {
                Id = game.Id,

                GameName = game.GamePlayed.GameName,

                User_01_Name = user01.UserName,
                User_01_Team = game.User_01_Team,
                GameScoreUser01 = game.GameScoreUser01,
                
                User_02_Name = user02.UserName,
                User_02_Team = game.User_02_Team,
                GameScoreUser02 = game.GameScoreUser02,
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteUserGame(UserGameListingModel model)
        {
            await _userGameService.DeleteUserGame(model.Id);

            // SignalR send message to All that DB was updated
            await _hubContext.Clients.All.SendAsync("Notify", $"Deleted UserGame at : {DateTime.Now}");

            return RedirectToAction("Index", "Home");
        }



        // =====================
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
            
            if(player1Score > player2Score)
            {
                winner = user1.Id;
            }

            if (player1Score < player2Score)
            {
                winner = user2.Id;
            }

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
                GamePlayed = gamePlayed
            };
        }
    }
}
