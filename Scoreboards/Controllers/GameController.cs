using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Scoreboards.Data;
using Scoreboards.Models.GamePage;
using Scoreboards.Models.UserGames;

namespace Scoreboards.Controllers
{
    public class GameController : Controller
    {
        private readonly IGame _game;
        private readonly IUserGame _userGameService;
        private readonly IApplicationUser _userService;

        public GameController(
              IGame game
            , IUserGame userGameService
            , IApplicationUser userService)
        {
            _game = game;
            _userGameService = userGameService;
            _userService = userService;
        }

        public IActionResult Index()
        {
            // Wrap them into the model
            var gameList = _game.GetAll();
            
            var model = new GameIndexModel
            {
                gameList = gameList
            };
            return View(model);
        }

        public IActionResult GameDetail(string gameId)
        {
            var game = _game.GetById(Int32.Parse(gameId));

            var MatchHistoryData = _userGameService.getUserGamesByGameName(game.GameName);

            IEnumerable<UserGameListingModel> GameSpecificMatchHistory = MatchHistoryData
                .OrderByDescending(g => g.GamePlayedOn)
                .Select((userGameItem) =>
                {
                    var userGame = _userGameService.GetById(userGameItem.Id);
                    var user_01 = _userService.GetById(userGame.User_01_Id);
                    var user_02 = _userService.GetById(userGame.User_02_Id);

                    UserGameListingModel model1 = new UserGameListingModel
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
                        GameScore = userGame.GameScore,

                        //Winner, “USER_01_Id”, “USER_02_Id”, “DRAW”
                        Winner = userGame.Winner,
                    };

                    return model1;
                });

            var model = new GameDetailModel
            {
                GameDetail = game,
                GameSpecificMatchHistory = GameSpecificMatchHistory
            };

            return View(model);
        }
    }
}