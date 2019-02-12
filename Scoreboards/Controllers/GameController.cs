using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Scoreboards.Data;
using Scoreboards.Data.Models;
using Scoreboards.Models.GamePage;
using Scoreboards.Models.UserGames;

namespace Scoreboards.Controllers
{
    [Authorize]
    public class GameController : Controller
    {
        private readonly IConfiguration _config;
        private readonly IGame _game;
        private readonly IUserGame _userGameService;
        private readonly IApplicationUser _userService;
        private readonly IMonthlyWinners _monthlyWinnersService;
        private readonly IUpload _uploadService;
        private readonly string _azureBlobStorageConnection;


        public GameController(
              IGame game
            , IUserGame userGameService
            , IApplicationUser userService
            , IMonthlyWinners monthlyWinnersService
            , IUpload uploadService
            , IConfiguration configuration)
        {
            _game = game;
            _userGameService = userGameService;
            _userService = userService;
            _monthlyWinnersService = monthlyWinnersService;
            _uploadService = uploadService;
            _config = configuration;
            // Game Images are in the same storage container as User Images, but they are in separate blobs
            _azureBlobStorageConnection = _config.GetConnectionString("AZURE_BLOB_STORAGE_USER_IMAGES");
        }

        /**
         * Redirects to Index (base) page
         */
         [AllowAnonymous]
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

        /**
         * Redirects to page which provides information on the games including a detailed match history
         */
        public IActionResult GameDetail(string gameId)
        {
            string currentChampion = _monthlyWinnersService.GetPastMonthWinnerWithGameId(gameId);
            ApplicationUser champion;
            var currentChampionName = "No Winner Last Month";
            
            // If champion exists and their profile was not deleted
            if (currentChampion != null)
            {
                champion = _userService.GetById(currentChampion);
                if (champion != null && !champion.IsProfileDeleted)
                {
                    currentChampionName = champion?.UserName;
                }
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

        /**
         * Redirects to a page which allows admins to add new games
         */
        [Authorize(Roles = "Admin")]
        public IActionResult AddGame()
        {
            var model = new NewGameModel
            {
                GameName = "",
                GameDescription = "",
                GameLogo = ""
            };
            
            return View(model);
        }

        /**
         * Calls a service method to add the input game
         */
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> AddNewGame(NewGameModel model)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("AddGame", "Game");
            }

            var game = BuildGame(model);

            await _game.AddGame(game);

            model.Id = game.Id;

            if (model.ImageUpload != null)
            {
                await UploadGameImageAsync(model);
            }

            return RedirectToAction("Index", "Game");
        }

        [Authorize(Roles = "Admin")]
        public IActionResult EditGame(int gameId)
        {
            var game = _game.GetById(gameId);

            var model = new NewGameModel
            {
                Id = gameId,
                GameDescription = game.GameDescription,
                GameLogo = game.GameLogo,
                GameName = game.GameName
            };


            return View(model);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> DeleteGame(NewGameModel model)
        {
            await _game.DeleteGame(model.Id);

            return RedirectToAction("Index", "Home");
        }

        /**
         * Calls a service method to edit the input game
         */
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> UpdateGame(NewGameModel model)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("EditGame", "Game");
            }

            var game = BuildGame(model);
            game.Id = model.Id;

            await _game.EditGame(game);

            return RedirectToAction("Index", "Game");
        }

        /**
        * Creates a game from the model to add to the database
        */
        private Game BuildGame(NewGameModel model)
        {
            return new Game
            {
                GameName = model.GameName.ToUpper(),
                GameLogo = model.GameLogo,
                // Points are put in the User Game Service, these values are simply
                // Placeholders
                WinPoints = 0,
                DrawPoints = 0,
                LossPoints = 0,
                GameDescription = model.GameDescription
            };
        }

        private async Task UploadGameImageAsync(NewGameModel model)
        {
            var game = _game.GetById(model.Id);
            var container = _uploadService.GetGameImagesBlobContainer(_azureBlobStorageConnection);

            var contentDisposition = ContentDispositionHeaderValue.Parse(model.ImageUpload.ContentDisposition);
            var fileName = contentDisposition.FileName.Trim('"');

            // Replace it with gameId to save space in Azure blob
            // As the file with the same name will overrite the old file.
            var fileExtension = fileName.Substring(fileName.LastIndexOf('.'));
            var gameIdFileName = String.Concat(model.Id, fileExtension);

            var blockBlob = container.GetBlockBlobReference(gameIdFileName);

            await blockBlob.UploadFromStreamAsync(model.ImageUpload.OpenReadStream());
            await _game.SetGameImageAsync(game, blockBlob.Uri);
        }
    }
}