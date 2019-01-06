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

        public async Task AddGame(Game game)
        {
            await _context.Games.AddAsync(game);
            await _context.SaveChangesAsync();
        }

        public IEnumerable<Game> GetAll()
        {
            return _context.Games;
        }

        public Game GetById(int gameId)
        {
            return GetAll().FirstOrDefault(game => game.Id == gameId);
        }

        public Game GetByName(string gameName)
        {
            return GetAll().FirstOrDefault(game => game.GameName == gameName);
        }

        public Task SetGameImageAsync(string uri)
        {
            throw new NotImplementedException();
        }

        public Task SetGameNameAsync(string gameName)
        {
            throw new NotImplementedException();
        }

    }
}
