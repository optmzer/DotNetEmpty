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

        public GameService(ApplicationDbContext context)
        {
            _context = context;
        }

        /**
         * Adds a game to the database
         * TODO: Remove or implement this feature.
         */
        public async Task AddGame(Game game)
        {
            await _context.Games.AddAsync(game);
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
