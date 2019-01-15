using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Scoreboards.Data;
using Scoreboards.Data.Models;
using Scoreboards.Models;
using Scoreboards.Models.Home;
using Scoreboards.Models.UserGames;
using Scoreboards.Models.Users;

namespace Scoreboards.Controllers
{
    public class HomeController : Controller
    {
        private readonly IUserGame _userGameService;
        private readonly IGame _gameService;
        private readonly IApplicationUser _userService;
        private readonly UserManager<ApplicationUser> _userManager;

        public HomeController(
              IUserGame userGameService
            , IGame gameService
            , IApplicationUser userService
            , UserManager<ApplicationUser> userManager)
        {
            _gameService = gameService;
            _userGameService = userGameService;
            _userManager = userManager;
            _userService = userService;
        }

        public IEnumerable<MatchHistoryModel> GetMatchDataWithId(string userId)
        {
            var history = _userGameService.GetAll()
                .Where(user => user.User_01_Id == userId || user.User_02_Id == userId)
                .Select(user => new MatchHistoryModel
                {
                    UserId = userId
                });
            return history;
        }

        public IActionResult Index(string gameName)
        {
            IEnumerable<LeaderboardUserModel> leaderBoardData;

            // prepare games list
            List<SelectListItem> listItems = new List<SelectListItem>();
            var gameList = _gameService.GetAll();
            foreach (var game in gameList)
            {
                listItems.Add(new SelectListItem
                {
                    Text = game.GameName,
                    Value = game.Id.ToString(),
                    Selected = false
                });
            }

            // prepare match history for specific game or overall game
            IEnumerable<UserGameListingModel> MatchHistoryList;

            if (gameName == "Overall" || gameName == null)
            {

                leaderBoardData = _userService
                    .GetAll()
                    .Select(user => new LeaderboardUserModel
                    {
                        UserId = user.Id,
                        UserName = user.UserName,
                        Wins = _userGameService.getWinsById(user.Id).ToString(),
                        Loses = _userGameService.getLosesById(user.Id).ToString(),
                        Ratio = _userGameService.getRatioWithId(user.Id).ToString(),
                        History = GetMatchDataWithId(user.Id)
                    });

                var MatchHistoryData = _userGameService.GetAll();

                MatchHistoryList = MatchHistoryData
                    .OrderByDescending(userGame => userGame.GamePlayedOn)
                    .Take(5)
                    .Select(userGameItem =>
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
                    })
                    ;
            }
            else
            {

                leaderBoardData = _userService.GetAll().Select(user => new LeaderboardUserModel
                {
                    UserId = user.Id,
                    UserName = user.UserName,
                    Wins = _userGameService.getWinsByIdAndGameName(user.Id, gameName).ToString(),
                    Loses = _userGameService.getLosesByIdAndGameName(user.Id, gameName).ToString(),
                    Ratio = _userGameService.getRatioWithIdAndGameName(user.Id, gameName).ToString(),
                    History = GetMatchDataWithId(user.Id)
                });

                leaderBoardData = leaderBoardData.Where(option => option.Wins + option.Loses != "00").Select(user => user);
                foreach (var item1 in listItems)
                {
                    if (item1.Value == gameName)
                    {
                        item1.Selected = true;
                    }
                    else
                    {
                        item1.Selected = false;
                    }
                }

                //var gameId = _userGameService.getUserGameByGameName(gameName);
                var MatchHistoryData = _userGameService.getUserGamesByGameName(gameName);

                MatchHistoryList = MatchHistoryData
                    .OrderByDescending(userGame => userGame.GamePlayedOn)
                    .Take(5)
                    .Select(userGameItem =>
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
            }

            var model = new HomeIndexModel
            {
                UsersData = leaderBoardData.OrderBy(user => decimal.Parse(user.Ratio)).Reverse(),
                // @lewis: LatestGames was MatchHistoryData from lewis's code
                LatestGames = MatchHistoryList,
                DropDownData = listItems,
                itemSelected = gameName == null ? "0" : gameName
            };
            // Display to the page
            return View(model);
        }

        [Route("notification")]
        public IActionResult Notify()
        {
            return RedirectToAction("Index", "Home");
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
