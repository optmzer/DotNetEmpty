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
        public List<SelectListItem> GetGameList(string gameId)
        {
            // prepare games list that will be used for dropdown
            // if gameName is provided => set that game to be 'Selected'
            // else => no 'Selected' game
            List<SelectListItem> listItems = new List<SelectListItem>();
            var gameList = _gameService.GetAll();
            foreach (var game in gameList)
            {
                var listItem = new SelectListItem
                {
                    Text = game.GameName,
                    Value = game.Id.ToString(),
                    Selected = false
                };
                if (gameId == listItem.Value)
                {
                    listItem.Selected = true;
                }
                listItems.Add(listItem);
            }
            return listItems;
        }
        public IEnumerable<LeaderboardUserModel> GetLeaderboardData(string gameId)
        {
            // prepare leaderboard
            // if gameName is provided => game specific leaderboard
            // else => overall leaderboard
            IEnumerable<LeaderboardUserModel> leaderBoardData = _userService.GetAll().Select(user => new LeaderboardUserModel
            {
                UserId = user.Id,
                ProfileImageUrl = user.ProfileImageUrl,
                UserName = user.UserName,
                Wins = _userGameService.getWinsByIdAndGameId(user.Id, gameId).ToString(),
                Draws = _userGameService.getDrawsByIdAndGameId(user.Id, gameId).ToString(),
                Loses = _userGameService.getLosesByIdAndGameId(user.Id, gameId).ToString(),
                Ratio = _userGameService.getRatioWithIdAndGameId(user.Id, gameId).ToString(),
                Points = _userGameService.getUserPoint(user.Id).ToString()
            });
            return leaderBoardData.Where(option => option.Wins + option.Loses != "00").Select(user => user);
        }
        public IEnumerable<UserGameListingModel> GetMatchHistory(string gameId)
        {
            // prepare match history for specific game or overall game
            IEnumerable<UserGameListingModel> MatchHistoryList = _userGameService
                .getUserGamesByGameId(gameId)
                .OrderByDescending((x) => x.GamePlayedOn)
                .Take(5)
                .Select((userGameItem) =>
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
            return MatchHistoryList;
        }
        public IActionResult Index(string gameId)
        {
            List<SelectListItem> listItems = GetGameList(gameId);
            IEnumerable<LeaderboardUserModel> leaderBoardData = GetLeaderboardData(gameId);
            IEnumerable<UserGameListingModel> MatchHistoryList = GetMatchHistory(gameId);
            
            var model = new HomeIndexModel
            {
                UsersData = leaderBoardData.OrderByDescending(user => int.Parse(user.Points))
                                .ThenByDescending(user => decimal.Parse(user.Ratio))
                                .ThenBy(user=> user.UserName),
                // @lewis: LatestGames was MatchHistoryData from lewis's code
                LatestGames = MatchHistoryList,
                DropDownData = listItems,
                itemSelected = gameId == null ? "0" : gameId
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
