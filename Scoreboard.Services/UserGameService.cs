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
        public int getWinsById(string userId)
        {
            return GetAll()
                .Where(userGame => 
                    (userGame.User_01_Id == userId || userGame.User_02_Id == userId) 
                    && userGame.Winner == userId)
                .Count();
        }
        public int getLosesById(string userId)
        {
            return GetAll()
                .Where(userGame => 
                    (userGame.User_01_Id == userId || userGame.User_02_Id == userId) 
                    && userGame.Winner != userId && userGame.Winner.ToLower() != "draw")
                .Count();
        }
        public decimal getRatioWithId(string userId)
        {
            int wins = getWinsById(userId);
            int loses = getLosesById(userId);
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
        public int getWinsByIdAndGameName(string userId, string gameName)
        {
            return GetAll()
                .Where(userGame => 
                    ((userGame.User_01_Id == userId || userGame.User_02_Id == userId) 
                    && userGame.Winner == userId) && gameName == userGame.GamePlayed.Id.ToString())
                .Count();
        }
        public int getLosesByIdAndGameName(string userId, string gameName)
        {
            return GetAll()
                .Where(userGame => 
                    ((userGame.User_01_Id == userId || userGame.User_02_Id == userId) 
                    && userGame.Winner != userId && userGame.Winner.ToLower() != "draw") 
                    && gameName == userGame.GamePlayed.Id.ToString())
                .Count();
        }
        public decimal getRatioWithIdAndGameName(string userId, string gameName)
        {
            int wins = getWinsByIdAndGameName(userId, gameName);
            int loses = getLosesByIdAndGameName(userId, gameName);
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
        public UserGame getUserGameByGameName(string gameName)
        {
            return GetAll()
                .Where(userGame => (userGame.GamePlayed.Id.ToString() == gameName))
                .FirstOrDefault();
        }

        public IEnumerable<UserGame> getUserGamesByUserId(string userId)
        {
            return GetAll()
                .Where(userGame => (userGame.User_01_Id == userId || userGame.User_02_Id == userId));
        }

        public IEnumerable<UserGame> getUserGamesByGameName(string gameName)
        {
            return GetAll().Where(userGame => (userGame.GamePlayed.GameName == gameName));
        }

        public IEnumerable<UserGame> getUserGamesByGameId(string gameId)
        {
            return GetAll().Where(userGame => userGame.Id == int.Parse(gameId));
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
