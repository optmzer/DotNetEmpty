using System.Collections.Generic;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Scoreboards.Data;
using Scoreboards.Data.Models;
using Scoreboards.Models.Home;
using Scoreboards.Models.UserGames;
using Scoreboards.Models.Users;

namespace Scoreboards.Controllers
{
    [Authorize]
    public class UsersController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IApplicationUser _userService;
        private readonly IUserGame _userGameService;
        private readonly IGame _gameService;
        private readonly IEmailSender _emailService;
        private readonly IMonthlyWinners _monthlyWinnersService;

        public UsersController(
            IApplicationUser userService,
            IUserGame userGameService,
            IGame gameService,
            IEmailSender emailService,
            UserManager<ApplicationUser> userManager,
            IMonthlyWinners monthlyWinnersService)
        {
            _userService = userService;
            _userGameService = userGameService;
            _gameService = gameService;
            _userManager = userManager;
            _emailService = emailService;
            _monthlyWinnersService = monthlyWinnersService;
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
            var MatchHistoryData = _userGameService.getUserGamesByUserId(userID);
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
                    GameScore = userGame.GameScoreUser01 + " : " + userGame.GameScoreUser02,

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
                    Wins = _userGameService.getWinsByIdAndGameId(user.Id, Game.Id.ToString()).ToString(),
                    Draws = _userGameService.getDrawsByIdAndGameId(user.Id, Game.Id.ToString()).ToString(),
                    Loses = _userGameService.getLosesByIdAndGameId(user.Id, Game.Id.ToString()).ToString(),
                    Ratio = _userGameService.getRatioWithIdAndGameId(user.Id, Game.Id.ToString()).ToString(),
                    MonthlyWins = _monthlyWinnersService.GetAllAwardsByUserIdAndGameId(user.Id, Game.Id.ToString())
                })).ToDictionary(x => x.Key, x => x.Value);

            gameStats.Add("Overall", new LeaderboardUserModel
            {
                UserId = user.Id,
                UserName = user.UserName,
                Wins = _userGameService.getWinsByIdAndGameId(user.Id, "").ToString(),
                Draws = _userGameService.getDrawsByIdAndGameId(user.Id, "").ToString(),
                Loses = _userGameService.getLosesByIdAndGameId(user.Id, "").ToString(),
                Ratio = _userGameService.getRatioWithIdAndGameId(user.Id, "").ToString(),
                MonthlyWins = _monthlyWinnersService.GetAllAwardsByUserIdAndGameId(user.Id, "Overall")
            });

            
            var model = new UserProfileModel
            {
                User = _userService.GetById(userID),
                UsersGames = MatchHistory,
                GameStatistcs = gameStats
            };

            return View(model);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Admin()
        {
            /**
             * Admin can edit profile
             * Delete profile
             * lockout profile - in case of illegal security breach
             */

            var model = new UsersModel
            {
                AppUsers = _userService.GetAll(),
                ListOfAdmins = _userService.GetByRole("Admin")
            };

            return View(model);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddUserToRole(string userId)
        {
            var user = _userService.GetById(userId);

            var result = await _userManager.AddToRoleAsync(user, "Admin");
            
            if (!result.Succeeded)
            {
                //Show some errors
            }

            return RedirectToAction("Admin", "Users");
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RemoveUserFromRole(string userId)
        {

            var user = _userService.GetById(userId);

            var result = await _userManager.RemoveFromRoleAsync(user, "Admin");

            if (!result.Succeeded)
            {
                //Show some errors
            }

            return RedirectToAction("Admin", "Users");
        }

        [Authorize(Roles = "Admin")]
        public IActionResult DeleteUser(string userId)
        {

            var model = new UserProfileModel
            {
                User = _userService.GetById(userId),
            };

            return View(model);
        }

        public async Task<IActionResult> DeletUserProfile(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            //await _userManager.RemoveFromRoleAsync(user, "Admin");
            await _userManager.DeleteAsync(user);

            return RedirectToAction("Admin", "Users");
        }

        public async Task<IActionResult> ResendEmailConfirmation(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if(user == null)
            {
                // TODO: Create a helper message usernot found
                return RedirectToAction("Admin", "Users");
            }

            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var callbackUrl = Url.Page(
                "/Account/ConfirmEmail",
                pageHandler: null,
                values: new { userId = user.Id, code = code },
                protocol: Request.Scheme);

            await _emailService.SendEmailAsync(user.Email, "Confirm your email at The Instillery Scoreboards",
                            $"Please confirm your account at The Instillery Scoreboards website by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");
            
            // Add message that confirmation email was sent to Email address
            return RedirectToAction("Admin", "Users");
        }

    }
}