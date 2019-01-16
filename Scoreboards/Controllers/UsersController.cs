using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Scoreboards.Data;
using Scoreboards.Data.Models;
using Scoreboards.Models.Home;
using Scoreboards.Models.UserGames;
using Scoreboards.Models.Users;

namespace Scoreboards.Controllers
{
    public class UsersController : Controller
    {
        private readonly IApplicationUser _userService;
        private readonly IUserGame _userGameService;
        private readonly IGame _gameService;

        public UsersController(IApplicationUser userService, IUserGame userGameService, IGame gameService)
        {
            _userService = userService;
            _userGameService = userGameService;
            _gameService = gameService;
        }

        public IActionResult Index()
        {
            var model = new UsersModel
            {
                AppUsers = _userService.GetAll()
            };
            // Display to the page
            return View(model);
        }

        public IActionResult Profile(string userID)
        {
            var MatchHistoryData = _userGameService.getUserGameById(userID);
            IEnumerable<UserGameListingModel> MatchHistory = MatchHistoryData.Select(userGame =>
            {
                UserGameListingModel ugameModel = new UserGameListingModel
                {
                    Id = userGame.Id,
                    //Game played Date
                    GamePlayedOn = userGame.GamePlayedOn,

                    //Players detail
                    User_01 = _userService.GetById(userGame.User_01_Id),
                    User_01_Team = userGame.User_01_Team,
                    User_02 = _userService.GetById(userGame.User_02_Id),
                    User_02_Team = userGame.User_02_Team,

                    // Game Name
                    GameName = userGame.GamePlayed.GameName,

                    //Score 
                    GameScore = userGame.GameScore,

                    //Winner, “USER_01_Id”, “USER_02_Id”, “DRAW”
                    Winner = userGame.Winner,
                };
                return ugameModel;
            });

            ApplicationUser user = _userService.GetById(userID);

            Dictionary<string, LeaderboardUserModel> gameStats = _gameService.GetAll()
                .Select(Game => new KeyValuePair<string, LeaderboardUserModel>(Game.GameName, new LeaderboardUserModel
                {
                    UserId = user.Id,
                    UserName = user.UserName,
                    Wins = _userGameService.getWinsByIdAndGameName(user.Id, Game.Id.ToString()).ToString(),
                    Loses = _userGameService.getLosesByIdAndGameName(user.Id, Game.Id.ToString()).ToString(),
                    Ratio = _userGameService.getRatioWithIdAndGameName(user.Id, Game.Id.ToString()).ToString()
                })).ToDictionary(x => x.Key, x => x.Value);

            gameStats.Add("Overall", new LeaderboardUserModel
            {
                UserId = user.Id,
                UserName = user.UserName,
                Wins = _userGameService.getWinsById(user.Id).ToString(),
                Loses = _userGameService.getLosesById(user.Id).ToString(),
                Ratio = _userGameService.getRatioWithId(user.Id).ToString()
            });

            var model = new UserProfileModel
            {
                User = _userService.GetById(userID),
                UsersGames = MatchHistory,
                GameStatistcs = gameStats

        };
            return View(model);
        }
    }
}