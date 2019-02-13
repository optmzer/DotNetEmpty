using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Scoreboards.Data;
using Scoreboards.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Scoreboards.Services
{
    public class GameService : IGame
    {
        private readonly IConfiguration _config;
        private readonly ApplicationDbContext _context;
        private readonly IUserGame _userGameServices;
        private readonly IUpload _uploadService;
        private readonly string AzureBlobStorageConnection;

        public GameService(IConfiguration configuration, ApplicationDbContext context, IUserGame userGameServices, IUpload uploadService)
        {
            _config = configuration;
            _context = context;
            _userGameServices = userGameServices;
            _uploadService = uploadService;
            AzureBlobStorageConnection = _config.GetConnectionString("AZURE_BLOB_STORAGE_USER_IMAGES");
        }

        /**
         * Adds a game to the database
         * TODO: Remove or implement this feature.
         */
        public async Task AddGame(Game game)
        {
            if (game.GameLogo == "" || game.GameLogo == null)
            {
                game.GameLogo = "/images/DefaultImage.png";
            }

            await _context.Games.AddAsync(game);
            await _context.SaveChangesAsync();
        }

        /**
         * Edits a game in the Database
         */
         public async Task EditGame(Game newGameContent)
         {
            var game = GetById(newGameContent.Id);
            // Mark for update
            _context.Entry(game).State = EntityState.Modified;
            game.GameName = newGameContent.GameName;
            game.GameDescription = newGameContent.GameDescription;
            game.GameLogo = newGameContent.GameLogo;

            if (game.GameLogo == "" || game.GameLogo == null)
            {
                game.GameLogo = "/images/DefaultImage.png";
            }

            await _context.SaveChangesAsync();
         }

        /**
         * Deletes a game in the Database
         */
         public async Task DeleteGame(int gameId)
        {
            var game = GetById(gameId);
            await _userGameServices.DeleteUserGamesForGame(game);
            await DeleteGameBlobImage(game.GameLogo);
            _context.Remove(game);
            await _context.SaveChangesAsync();
        }

        /**
         * Removes the to be deleted games image from the blob storage to conserve storage room
         */
        private async Task DeleteGameBlobImage(string imageUri)
        {
            var blobStorageContainer = _uploadService.GetGameImagesBlobContainer(AzureBlobStorageConnection);
            // Service client is used to get the reference because we have the Uri of the image not its name within the 
            // container
            var blobImage = await blobStorageContainer.ServiceClient.GetBlobReferenceFromServerAsync(new Uri(imageUri));
            await blobImage.DeleteAsync();
        }

        /**
         * Returns a list of all games in the database
         */
        public IEnumerable<Game> GetAll()
        {
            return _context.Games;
        }

        /**
         * Returns the game identified by the input Id
         */
        public Game GetById(int gameId)
        {
            return GetAll().FirstOrDefault(game => game.Id == gameId);
        }

        /**
         * Returns the game identified by the input name
         */
        public Game GetByName(string gameName)
        {
            return GetAll().FirstOrDefault(game => game.GameName == gameName);
        }

        /**
         * Changes the Game Image to the selected input
         * TODO: implement or remove
         */
        public async Task SetGameImageAsync(Game game, Uri uri)
        {
            game.GameLogo = uri.AbsoluteUri;

            _context.Update(game);
            await _context.SaveChangesAsync();
        }

        /**
         * Changes the Game name to the selected input
         * TODO: implement or remove
         */
        public Task SetGameNameAsync(string gameName)
        {
            throw new NotImplementedException();
        }

    }
}
