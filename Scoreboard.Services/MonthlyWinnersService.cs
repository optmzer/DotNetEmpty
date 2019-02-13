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
    public class MonthlyWinnersService : IMonthlyWinners
    {
        private readonly ApplicationDbContext _context;
        private readonly IUserGame _userGameServices;
        public MonthlyWinnersService(ApplicationDbContext context, IUserGame userGameServices)
        {
            _context = context;
            _userGameServices = userGameServices;
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
         * Returns the awards won by a user in a specified game in the overall month
         * Returns a IEnumerable list containing only one item.
         */
        /** if we want to display all the past month winners on all time tab  */
        //public IEnumerable<MonthlyWinners> GetPastMonthAwardWithIdAndGameId(string userId, string gameId)
        //{
        //    if (gameId == "" || gameId == null || gameId == "0" || gameId.ToLower() == "overall")
        //    {
        //        return _context.MonthlyWinners
        //            .Where(award =>
        //                   award.WinnerId == userId
        //                   && award.GamePlayedId.ToLower() == "overall");
        //    } else
        //    {
        //        return _context.MonthlyWinners
        //            .Where(award =>
        //                   award.WinnerId == userId
        //                   && award.GamePlayedId.ToLower() == gameId.ToLower());
        //    }
        //}
        /** if we want to display all the past month winners on all time tab  */
        public IEnumerable<MonthlyWinners> GetPastMonthAwardWithIdAndGameId(string userId, string gameId)
        {
            if (gameId == "" || gameId == null || gameId == "0" || gameId.ToLower() == "overall")
            {
                return _context.MonthlyWinners
                    .Where(award =>
                           award.WinnerId == userId
                           && award.GamePlayedId.ToLower() == "overall"
                           && award.RecordedDate.AddMonths(-1).Month == DateTime.Now.AddMonths(-1).Month);
            }
            else
            {
                return _context.MonthlyWinners
                    .Where(award =>
                           award.WinnerId == userId
                           && award.GamePlayedId.ToLower() == gameId.ToLower()
                           && award.RecordedDate.AddMonths(-1).Month == DateTime.Now.AddMonths(-1).Month);
            }
        }


        /**
         * Overloaded GetPastMonthAwardWithIdAndGameId method to improve optimisation, prevents the need to get all
         * the monthly winners again, reducing load time.
         * 
         * Returns the awards won by a user in a specified game in the specified month
         * Returns a IEnumerable list containing only one item.
         */
        public IEnumerable<MonthlyWinners> GetPastMonthAwardWithIdAndGameId(string userId, string gameId, DateTime monthFetched)
        {
            //if (monthFetched.Month == DateTime.Now.Month)
            //{   // when month set is current month, it returns previous month's winner
            //    monthFetched = monthFetched.AddMonths(-1);
            //}
            if (gameId =="" || gameId==null || gameId == "0" || gameId.ToLower() == "overall")
            {
                return _context.MonthlyWinners
                    .Where(award => 
                           award.WinnerId == userId 
                           && award.GamePlayedId.ToLower() == "overall"
                           && award.RecordedDate.AddMonths(-1).Month == monthFetched.Month);
            } else
            {
                return _context.MonthlyWinners
                    .Where(award => 
                           award.WinnerId == userId 
                           && award.GamePlayedId == gameId
                           && award.RecordedDate.AddMonths(-1).Month == monthFetched.Month);
            }
        }


        /**
         * Overloaded GetPastMonthAwardWithIdAndGameId method to improve optimisation, prevents the need to get all
         * the monthly winners again, reducing load time.
         * 
         * Returns the awards won by a user in a specified game in the overall month
         * Returns a IEnumerable list containing only one item.
         */

        /** if we want to display all the past month winners on all time tab  */
        //public IEnumerable<MonthlyWinners> GetPastMonthAwardWithIdAndGameId(IEnumerable<MonthlyWinners> monthlyWinners, string userId, string gameId)
        //{
        //    if (gameId == "" || gameId == null || gameId == "0" || gameId.ToLower() == "overall")
        //    {
        //        return monthlyWinners
        //            .Where(award =>
        //                   award.WinnerId == userId
        //                   && award.GamePlayedId.ToLower() == "overall");
        //    }
        //    else
        //    {
        //        return monthlyWinners
        //            .Where(award =>
        //                   award.WinnerId == userId
        //                   && award.GamePlayedId.ToLower() == gameId.ToLower());
        //    }
        //}

        /** if we want to display previous month winner only on all time tab  */
        public IEnumerable<MonthlyWinners> GetPastMonthAwardWithIdAndGameId(IEnumerable<MonthlyWinners> monthlyWinners, string userId, string gameId)
        {
            if (gameId == "" || gameId == null || gameId == "0" || gameId.ToLower() == "overall")
            {
                return monthlyWinners
                    .Where(award =>
                           award.WinnerId == userId
                           && award.GamePlayedId.ToLower() == "overall"
                           && award.RecordedDate.AddMonths(-1).Month == DateTime.Now.AddMonths(-1).Month);
            }
            else
            {
                return monthlyWinners
                    .Where(award =>
                           award.WinnerId == userId
                           && award.GamePlayedId.ToLower() == gameId.ToLower()
                           && award.RecordedDate.AddMonths(-1).Month == DateTime.Now.AddMonths(-1).Month);
            }
        }


        /**
         * Overloaded GetPastMonthAwardWithIdAndGameId method to improve optimisation, prevents the need to get all
         * the monthly winners again, reducing load time.
         * 
         * Returns the awards won by a user in a specified game in the specified month
         * Returns a IEnumerable list containing only one item.
         */
        public IEnumerable<MonthlyWinners> GetPastMonthAwardWithIdAndGameId(IEnumerable<MonthlyWinners> monthlyWinners, string userId, string gameId, DateTime monthFetched)
        {
            //if (monthFetched.Month == DateTime.Now.Month)
            //{   // when month set is current month, it returns previous month's winner
            //    monthFetched = monthFetched.AddMonths(-1);
            //}
            if (gameId == "" || gameId == null || gameId.ToLower() == "overall")
            {
                return monthlyWinners
                    .Where(award =>
                           award.WinnerId == userId 
                           && award.GamePlayedId.ToLower() == "overall" 
                           && award.RecordedDate.AddMonths(-1).Month == monthFetched.Month);
            }
            else
            {
                return monthlyWinners
                    .Where(award => 
                           award.WinnerId == userId 
                           && award.GamePlayedId == gameId
                           && award.RecordedDate.AddMonths(-1).Month == monthFetched.Month);
            }
        }

        public string GetPastMonthWinnerWithGameId(string gameId)
        {
            string past_month = DateTime.Now.AddMonths(-1).ToString("MMMM yyyy");

            if (gameId == "" || gameId == null || gameId.ToLower() == "overall")
            {
                return _context.MonthlyWinners
                           .Where(award =>
                           award.GamePlayedId.ToLower() == "overall"
                           && award.Title.Contains(past_month))
                           .Select(award => award.WinnerId).FirstOrDefault();
            }
            else
            {
                return _context.MonthlyWinners
                           .Where(award =>
                           award.GamePlayedId.ToLower() == gameId.ToLower()
                           && award.Title.Contains(past_month))
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
            if (gameId == null || gameId == "" || gameId.ToLower() == "overall")
            {
                gameId = "overall";
            }
            var WinnerId = _userGameServices.GetLastMonthWinner(gameId);
            MonthlyWinners newWinner = new MonthlyWinners()
            {
                Title = time.AddMonths(-1).ToString("MMMM") + " " + time.AddMonths(-1).Year + " Champion",
                GamePlayedId = gameId,
                WinnerId = WinnerId,
                RecordedDate = time.AddMonths(-1)
            };
            

            _context.Add(newWinner);
            await _context.SaveChangesAsync(); // commits changes to DB.
        }
    }
}
