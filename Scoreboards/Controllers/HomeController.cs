using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Scoreboards.Data;
using Scoreboards.Data.Models;
using Scoreboards.Models;
using Scoreboards.Models.Home;
using Scoreboards.Models.UserGames;

namespace Scoreboards.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly IUserGame _userGameService;
        private readonly IGame _gameService;
        private readonly IApplicationUser _userService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMonthlyWinners _monthlyWinnersService;

        public HomeController(
              IUserGame userGameService
            , IGame gameService
            , IApplicationUser userService
            , UserManager<ApplicationUser> userManager
            , IMonthlyWinners monthlyWinnersService)
        {
            _gameService = gameService;
            _userGameService = userGameService;
            _userManager = userManager;
            _userService = userService;
            _monthlyWinnersService = monthlyWinnersService;
        }

        [AllowAnonymous]
        public IActionResult Index(string gameId, string month)
        {
            List<SelectListItem> listItems = GetGameList(gameId);
            IEnumerable<UserGameListingModel> MatchHistoryList = GetMatchHistory(gameId);
            IEnumerable<LeaderboardUserModel> leaderboardData = prepareLeaderBoard(gameId);
            List<SelectListItem> months = GetMonthList(month);

            var model = new HomeIndexModel
            {
                UsersData = leaderboardData
                                .OrderByDescending(user => int.Parse(user.Points))
                                .ThenByDescending(user => decimal.Parse(user.Ratio))
                                .ThenBy(user=> user.UserName),
                // @lewis: LatestGames was MatchHistoryData from lewis's code
                LatestGames = MatchHistoryList,
                DropDownData = listItems,
                itemSelected = gameId == null ? "0" : gameId,
                DropDownSeasons = months,
                monthSelected = month == null ? "0" : month
            };
            // Display to the page
            return View(model);
        }
        /**
         * Used to send buuble type notifications to the page from SignalR
         * Currently it only refreshes the page.
         *  connection.on("Notify", function (message) {
         *  //reload the page
         *      location.reload();
         *  });
         */
        [AllowAnonymous]
        [Route("notification")]
        public IActionResult Notify()
        {
            return RedirectToAction("Index", "Home");
        }

        [AllowAnonymous]
        public IActionResult EmailConfirmation()
        {
            return View();
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
        private IEnumerable<LeaderboardUserModel> prepareLeaderBoard(string gameId)
        {

            IEnumerable<UserGame> userGameList = _userGameService.GetAll();
            IEnumerable<MonthlyWinners> monthlyWinners = _monthlyWinnersService.GetAll();
            IEnumerable<LeaderboardUserModel> leaderboardData = _userService.GetAllActive()
                .Select(user => {
                int wins = _userGameService.getWinsByIdAndGameId(userGameList, user.Id, gameId);
                int losses = _userGameService.getLosesByIdAndGameId(userGameList, user.Id, gameId);
                int draws = _userGameService.getDrawsByIdAndGameId(userGameList, user.Id, gameId);
                IEnumerable<UserGame> userSpecificUGList = userGameList.Where(userGame =>
                    (userGame.User_01_Id.ToString() == user.Id)
                    || userGame.User_02_Id.ToString() == user.Id);

                return new LeaderboardUserModel
                {
                    UserId = user.Id,
                    ProfileImageUrl = user.ProfileImageUrl,
                    UserName = user.UserName,
                    Wins = wins.ToString(),
                    Draws = draws.ToString(),
                    Loses = losses.ToString(),
                    Ratio = _userGameService.getRatioWithIdAndGameId(wins, losses).ToString(),
                    Points = _userGameService.getUserPoint(userSpecificUGList, user.Id, gameId).ToString(),
                    MonthlyWins = _monthlyWinnersService.GetPastMonthAwardWithIdAndGameId(monthlyWinners, user.Id, gameId)
                };
            });

            return leaderboardData;
        }
        private List<SelectListItem> GetGameList(string gameId)
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
        private IEnumerable<UserGameListingModel> GetMatchHistory(string gameId)
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
        private List<SelectListItem> GetMonthList(string month)
        {
            // prepare games list that will be used for dropdown
            // if gameName is provided => set that game to be 'Selected'
            // else => no 'Selected' game
            List<SelectListItem> listItems = new List<SelectListItem>();
            var monthList = _monthlyWinnersService.GetAllMonths();
            foreach (var selectedMonth in monthList)
            {
                var listItem = new SelectListItem
                {
                    Text = selectedMonth.Replace(" Champion", ""),
                    Value = selectedMonth.Replace(" Champion", ""),
                    Selected = false
                };
                if (month == listItem.Value)
                {
                    listItem.Selected = true;
                }
                listItems.Add(listItem);
            }
            return listItems;
        }
    }
}
