﻿using Microsoft.EntityFrameworkCore;
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
    public class MonthlyWinnersService : IMonthlyWinners
    {
        private readonly ApplicationDbContext _context;
        private readonly UserGameService _userGameContext;
        public MonthlyWinnersService(ApplicationDbContext context, UserGameService userGameContext)
        {
            _context = context;
            _userGameContext = userGameContext;
        }

        /**
         * Returns a list of all previous monthly winners
         */
        public IEnumerable<MonthlyWinners> GetAll()
        {
            return _context.MonthlyWinners
                    .OrderBy(award => award.RecordedDate);
        }

        /**
         * Returns a list of all monthly awards won by the input user
         * ordered by recorded date
         */
        public IEnumerable<MonthlyWinners> GetAllAwardsByUserId(string userId)
        {
            return _context.MonthlyWinners
                    .Where(award => award.WinnerId == userId)
                    .OrderBy(award => award.RecordedDate);
        }

        /**
         * Returns a list of all monthly awards won by the input user in a specified game
         */
        public IEnumerable<MonthlyWinners> GetAllAwardsByUserIdAndGameId(string userId, string gameId)
        {
            return _context.MonthlyWinners
                    .Where(award => 
                           award.WinnerId == userId 
                           && award.GamePlayedId.ToLower() == gameId.ToLower())
                    .OrderBy(award => award.RecordedDate);
        }

        /**
         * Returns the awards won by a user in a specified game in the previous month
         * Returns a IEnumerable list containing only one item.
         */
        public IEnumerable<MonthlyWinners> GetPastMonthAwardWithIdAndGameId(string userId, string gameId)
        {
            if (gameId =="" || gameId==null || gameId.ToLower() == "overall")
            {
                return _context.MonthlyWinners
                    .Where(award => 
                           award.WinnerId == userId 
                           && award.GamePlayedId.ToLower() == "overall" 
                           && DateTime.Now.AddMonths(-1).Month == award.RecordedDate.Month);
            } else
            {
                return _context.MonthlyWinners
                    .Where(award => 
                           award.WinnerId == userId 
                           && award.GamePlayedId.ToLower() == gameId.ToLower() 
                           && DateTime.Now.AddMonths(-1).Month == award.RecordedDate.Month);
            }
        }

        /**
         * Overloaded GetPastMonthAwardWithIdAndGameId method to improve optimisation, prevents the need to get all
         * the monthly winners again, reducing load time.
         * 
         * Returns the awards won by a user in a specified game in the previous month
         * Returns a IEnumerable list containing only one item.
         */
        public IEnumerable<MonthlyWinners> GetPastMonthAwardWithIdAndGameId(IEnumerable<MonthlyWinners> monthlyWinners, 
                                                                            string userId, 
                                                                            string gameId)
        {
            if (gameId == "" || gameId == null || gameId.ToLower() == "overall")
            {
                return monthlyWinners
                    .Where(award =>
                           award.WinnerId == userId 
                           && award.GamePlayedId.ToLower() == "overall" 
                           && DateTime.Now.AddMonths(-1).Month == award.RecordedDate.Month);
            }
            else
            {
                return monthlyWinners
                    .Where(award => 
                           award.WinnerId == userId 
                           && award.GamePlayedId.ToLower() == gameId.ToLower() 
                           && DateTime.Now.AddMonths(-1).Month == award.RecordedDate.Month);
            }
        }

        public string GetPastMonthWinnerWithGameId(string gameId)
        {
            if (gameId == "" || gameId == null || gameId.ToLower() == "overall")
            {
                return _context.MonthlyWinners
                           .Where(award =>
                           award.GamePlayedId.ToLower() == "overall"
                           && DateTime.Now.AddMonths(-1).Month == award.RecordedDate.Month)
                           .Select(award => award.WinnerId).FirstOrDefault();
            }
            else
            {
                return _context.MonthlyWinners
                           .Where(award =>
                           award.GamePlayedId.ToLower() == gameId.ToLower()
                           && DateTime.Now.AddMonths(-1).Month == award.RecordedDate.Month)
                           .Select(award => award.WinnerId).FirstOrDefault();
            }
        }

        /**
         * Returns all the months which awards have been given out for. Used to display in the dropdown menu
         * and for filtering the home page scoreboard.
         */
        public List<string> GetAllMonths()
        {
            var awardsList = _context.MonthlyWinners.Select(award => 
                                                            award.Title);
            // This line isn't working the change was instead made in the home controller
            //awardsList.ToList().ForEach(title => 
            //                            title = title.Replace(" Champion", ""));
            return awardsList.ToList();
        }

        public async Task AddNewWinnerAsync(string gameId)
        {
            /**
            * Entity framwork handls all logic for us
            * all we need to do is to call _context.Add() method
            * and EntityFramwork will figure out where to stick it.
            */
            var time = DateTime.Now;
            var WinnerId = _userGameContext.GetLastMonthWinner(gameId);
            MonthlyWinners newWinner = new MonthlyWinners()
            {
                Title = time.Month.ToString("MMMM") + " " + time.Year,
                GamePlayedId = gameId,
                WinnerId = WinnerId,
                RecordedDate = time
            };
            

            _context.Add(newWinner);
            await _context.SaveChangesAsync(); // commits changes to DB.
        }
    }
}
