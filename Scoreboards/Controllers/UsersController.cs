using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using System.Web;
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
                AppUsers = _userService.GetAllActive()
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

                    // Score 
                    GameScore = userGame.GameScoreUser01 + " : " + userGame.GameScoreUser02,

                    // Winner, “USER_01_Id”, “USER_02_Id”, “DRAW”
                    Winner = userGame.Winner,
                };
                return ugameModel;
            }).OrderByDescending(game => game.Id);

            ApplicationUser user = _userService.GetById(userID);
            IEnumerable<UserGame> userGameList = _userGameService.GetAll();
            Dictionary<string, LeaderboardUserModel> gameStats = _gameService.GetAll()
                .Select(Game => new KeyValuePair<string, LeaderboardUserModel>(Game.GameName, new LeaderboardUserModel
                {
                    UserId = user.Id,
                    UserName = user.UserName,
                    Wins = _userGameService.getWinsByIdAndGameId(userGameList, user.Id, Game.Id.ToString()).ToString(),
                    Draws = _userGameService.getDrawsByIdAndGameId(userGameList, user.Id, Game.Id.ToString()).ToString(),
                    Loses = _userGameService.getLosesByIdAndGameId(userGameList, user.Id, Game.Id.ToString()).ToString(),
                    Ratio = _userGameService.getRatioWithIdAndGameId(userGameList, user.Id, Game.Id.ToString()).ToString(),
                    MonthlyWins = _monthlyWinnersService.GetPastMonthAwardWithIdAndGameId(user.Id, Game.Id.ToString())
                })).ToDictionary(x => x.Key, x => x.Value);

            gameStats.Add("Overall", new LeaderboardUserModel
            {
                UserId = user.Id,
                UserName = user.UserName,
                Wins = _userGameService.getWinsByIdAndGameId(userGameList, user.Id, "").ToString(),
                Draws = _userGameService.getDrawsByIdAndGameId(userGameList, user.Id, "").ToString(),
                Loses = _userGameService.getLosesByIdAndGameId(userGameList, user.Id, "").ToString(),
                Ratio = _userGameService.getRatioWithIdAndGameId(userGameList, user.Id, "").ToString(),
                MonthlyWins = _monthlyWinnersService.GetPastMonthAwardWithIdAndGameId(user.Id, "overall")
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
        public IActionResult Admin(string message = "")
        {
            /**
             * Admin can edit profile
             * Delete profile
             * lockout profile - in case of illegal security breach
             */

            var model = new UsersModel
            {
                AppUsers = _userService.GetAll().OrderBy(user => user.UserName),
                ListOfAdmins = _userService.GetByRole("Admin").OrderBy(user => user.UserName),
                StatusMessage = message
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
        public IActionResult AdminActions(string userId)
        {
            var user = _userService.GetById(userId);
            var IsEmailConfirmed = _userManager.IsEmailConfirmedAsync(user).Result;

            var model = new UserProfileModel
            {
                User = user,
                IsEmailConfirmed = IsEmailConfirmed
            };

            return View(model);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteUserProfile(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            await _userService.DeleteUserProfileAsync(user);

            return RedirectToAction("Admin", "Users");
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> ResendEmailConfirmation(UserProfileModel model)
        {
            var userId = model.User.Id;
            var user = _userService.GetById(userId);

            if(user == null)
            {
                // TODO: Create a helper message usernot found
                return RedirectToAction("Admin", "Users");
            }

            if(user.Email != model.User.Email)
            {
                await _userManager.SetEmailAsync(user, model.User.Email);
            }

            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);

            // Create callbackUrl
            var protocol = HttpContext.Request.Scheme;
            var host = HttpContext.Request.Host;
            var queryString = "?userId=" + HttpUtility.UrlEncode(user.Id) + "&code=" + HttpUtility.UrlEncode(code);
            var callbackUrl = protocol + "://" + host + "/Identity/Account/ConfirmEmail" + queryString;

            await _emailService.SendEmailAsync(user.Email, "Confirm your email at The Instillery Scoreboards",
                            $"Please confirm your account at The Instillery Scoreboards website by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");
            
            // Add message that confirmation email was sent to Email address
            return RedirectToAction("Admin", "Users");
        }

        [Authorize(Roles="Admin")]
        public IActionResult ResetStats(string message = "")
        {
            ICollection<DateTime> monthNames = new List<DateTime>();
            for (int i = 11; i >= 0; --i)
            {
                monthNames.Add(DateTime.Now.AddMonths(-i));
            }

            var model = new ResetStatsModel
            {
                MonthNames = monthNames,
                StatusMessage = message
            };

            return View(model);
        }

        [Authorize(Roles ="Admin")]
        [HttpPost]
        public async Task<IActionResult> DeleteUserGameHistory(ResetStatsModel model)
        {
            var message = "Action Was Canceled. There Were No Changes Made To The Database";

            if (model.MonthSelected == null)
            {
                await _userGameService.DeleteAllUserGames();
                message = "All Games Were Deleted Successfully.";
            }

            if(model.MonthSelected.Year > 1)
            {
                await _userGameService.DeleteUserGameByMonth(model.MonthSelected);
                message = "Games For " + model.MonthSelected.ToString("MMMM yyyy") + " Were Deleted Successfully.";
            }
            return RedirectToAction("ResetStats", "Users", new { message });
        }
    }
}