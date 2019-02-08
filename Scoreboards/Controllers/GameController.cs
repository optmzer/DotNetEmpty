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
        private readonly IMonthlyWinners _monthlyWinnersService;

        public GameController(
              IGame game
            , IUserGame userGameService
            , IApplicationUser userService
            , IMonthlyWinners monthlyWinnersService)
        {
            _game = game;
            _userGameService = userGameService;
            _userService = userService;
            _monthlyWinnersService = monthlyWinnersService;
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
            string currentChampion = _monthlyWinnersService.GetPastMonthWinnerWithGameId(gameId);
            var currentChampionName = "No Winner Last Month";
            if (currentChampion != null)
            {
                currentChampionName = _userService.GetById(currentChampion).UserName;
            }
            var game = _game.GetById(Int32.Parse(gameId));
            var MatchHistoryData = _userGameService.getUserGamesByGameId(gameId);
            IEnumerable<UserGameListingModel> GameSpecificMatchHistory = MatchHistoryData.OrderByDescending((x)=> x.GamePlayedOn).Select((userGameItem) =>
            {
                UserGameListingModel model1 = new UserGameListingModel
                {
                    Id = userGameItem.Id,
                    //Game played Date
                    GamePlayedOn = userGameItem.GamePlayedOn,

                    //Players detail
                    User_01 = _userService.GetById(userGameItem.User_01_Id),
                    User_01_Team = userGameItem.User_01_Team,
                    User_02 = _userService.GetById(userGameItem.User_02_Id),
                    User_02_Team = userGameItem.User_02_Team,

                    // Game Name
                    GameName = userGameItem.GamePlayed.GameName,

                    //Score 
                    GameScore = userGameItem.GameScoreUser01 + " : " + userGameItem.GameScoreUser02,

                    //Winner, “USER_01_Id”, “USER_02_Id”, “DRAW”
                    Winner = userGameItem.Winner,
                };
                return model1;
            });

            var model = new GameDetailModel
            {
                GameDetail = game,
                GameSpecificMatchHistory = GameSpecificMatchHistory,
                ReigningChampion = currentChampionName
            };

            return View(model);
        }
    }
}