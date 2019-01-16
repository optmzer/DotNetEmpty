using Microsoft.EntityFrameworkCore;
using Scoreboards.Data;
using Scoreboards.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

/*
 * Updates UserGame data
 */
namespace Scoreboards.Services
{
    public class UserGameService : IUserGame
    {
        private readonly ApplicationDbContext _context;
        public UserGameService(ApplicationDbContext context)
        {
            _context = context;
        }

        public IEnumerable<UserGame> GetAll()
        {
            return _context.UserGames.Include(game => game.GamePlayed);
        }

        public IEnumerable<UserGame> GetLatest(int numberOfGames)
        {
            return GetAll()
                .OrderByDescending(userGame => userGame.GamePlayedOn)
                .Take(numberOfGames);
        }

        // Lewis added
        public int getWinsByIdAndGameId(string userId, string gameId)
        {
            // if gameName is null, provide overall
            // else provide overall
            if (gameId == null || gameId == "")
            {
                return GetAll().Where(userGame => (userGame.User_01_Id == userId || userGame.User_02_Id == userId) && userGame.Winner == userId).Count();
            } else
            {
                return GetAll().Where(userGame => ((userGame.User_01_Id == userId || userGame.User_02_Id == userId) && userGame.Winner == userId) && gameId == userGame.GamePlayed.Id.ToString()).Count();
            }
        }
        public int getDrawsByIdAndGameId(string userId, string gameId)
        {
            // if gameName is null, provide overall
            // else provide overall
            if (gameId == null || gameId == "")
            {
                return GetAll().Where(userGame => (userGame.User_01_Id == userId || userGame.User_02_Id == userId) && userGame.Winner.ToLower() == "draw").Count();
            }
            else
            {
                return GetAll().Where(userGame => ((userGame.User_01_Id == userId || userGame.User_02_Id == userId) && userGame.Winner.ToLower() == "draw") && gameId == userGame.GamePlayed.Id.ToString()).Count();
            }
        }
        public int getLosesByIdAndGameId(string userId, string gameId)
        {
            // if gameName is null, provide overall
            // else provide overall
            if (gameId == null || gameId == "")
            {
                return GetAll().Where(userGame => (userGame.User_01_Id == userId || userGame.User_02_Id == userId) && userGame.Winner != userId && userGame.Winner.ToLower() != "draw").Count();
            }
            else
            {
                return GetAll().Where(userGame => ((userGame.User_01_Id == userId || userGame.User_02_Id == userId) && userGame.Winner != userId && userGame.Winner.ToLower() != "draw") && gameId == userGame.GamePlayed.Id.ToString()).Count();
            }
        }
        public decimal getRatioWithIdAndGameId(string userId, string gameId)
        {
            int wins = getWinsByIdAndGameId(userId, gameId);
            int loses = getLosesByIdAndGameId(userId, gameId);
            int total = wins + loses;
            if (total == 0)
            {
                return 0;
            }
            else
            {
                decimal ratio = (decimal)wins / (decimal)(total) * 100;
                return Math.Round(ratio, 2);
            }
        }
        public decimal getRatioIncludingDrawWithIdAndGameId(string userId, string gameId)
        {
            int wins = getWinsByIdAndGameId(userId, gameId);
            int draws = getDrawsByIdAndGameId(userId, gameId);
            int loses = getLosesByIdAndGameId(userId, gameId);
            int total = wins + draws + loses;
            if (total == 0)
            {
                return 0;
            }
            else
            {
                decimal ratio = (decimal)wins / (decimal)(total) * 100;
                return Math.Round(ratio, 2);
            }
        }
        public IEnumerable<UserGame> getUserGamesByGameId(string gameId)
        {
            if (gameId==null||gameId=="")
            {
                return GetAll();
            } else
            {
                return GetAll().Where(userGame => (userGame.GamePlayed.Id.ToString() == gameId));
            }
            
        }

        ///////////////////////////////////////////

        public UserGame GetById(int userGameId)
        {
            return GetAll().FirstOrDefault(userGame => userGame.Id == userGameId);
        }

        public async Task AddUserGameAsync(UserGame userGame)
        {
            /**
            * Entity framwork handls all logic for us
            * all we need to do is to call _context.Add() method
            * and EntityFramwork will figure out where to stick it.
            */
            _context.Add(userGame);
            await _context.SaveChangesAsync(); // commits changes to DB.
        }

        public async Task Delete(int userGameId)
        {
            var userGame = GetById(userGameId);

            _context.Remove(userGame);
            await _context.SaveChangesAsync(); // commits changes to DB.
        }

        public async Task EditUserGame(UserGame newUserGameContent)
        {
            _context.Entry(newUserGameContent).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public Task SetGameImageAsync(int userGameId, Uri uri)
        {
            throw new NotImplementedException();
        }

        public int CountWinsForUser(string userId)
        {
            return GetAll().Where(uGame => uGame.Winner == userId).Count();
        }

    }
}
