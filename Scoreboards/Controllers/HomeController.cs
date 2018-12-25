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
        private readonly IApplicationUser _userService;
        private readonly UserManager<ApplicationUser> _userManager;

        public HomeController(
              IUserGame userGameService
            , IApplicationUser userService
            , UserManager<ApplicationUser> userManager)
        {
            _userGameService = userGameService;
            _userManager = userManager;
            _userService = userService;
        }

        public IActionResult Index()
        {
            // Get All Games
            var games = _userGameService.GetLatest(5);

            // Wrap them into the model
            var gameLising = games.Select(game => new UserGameListingModel
            {
                Id = game.Id,
                GamePlayedOn = game.GamePlayedOn,
                User_01 = _userService.GetById(game.User_01_Id),
                User_01_Team = game.User_01_Team,
                User_02 = _userService.GetById(game.User_02_Id),
                User_02_Team = game.User_02_Team,

                // Game Name
                GameName = game.GamePlayed.GameName,

                //Score 
                GameScore = game.GameScore,

                //Winner Id
                Winner = game.Winner,
                //Referee
                Referee = _userService.GetById(game.RefereeUserId)
            });

            var model = new HomeIndexModel
            {
                LatestGames = gameLising,
                //TODO: SortBy
                //TODO: ListOfTopPlayers by rating
                SearchQuery = ""
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
