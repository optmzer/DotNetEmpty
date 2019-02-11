using Microsoft.EntityFrameworkCore;
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
        private readonly ApplicationDbContext _context;
        private readonly IUserGame _userGameServices;

        public GameService(ApplicationDbContext context, IUserGame userGameServices)
        {
            _context = context;
            _userGameServices = userGameServices;
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
            _context.Remove(game);
            await _context.SaveChangesAsync();
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
        public Task SetGameImageAsync(string uri)
        {
            throw new NotImplementedException();
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
