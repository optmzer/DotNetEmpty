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
        public MonthlyWinnersService(ApplicationDbContext context)
        {
            _context = context;
        }
        public IEnumerable<MonthlyWinners> GetAllAwardsByUserId(string userId)
        {
            return _context.MonthlyWinners
                    .Where(award => award.WinnerId == userId)
                    .OrderBy(award => award.RecordedDate);
        }
        public IEnumerable<MonthlyWinners> GetAllAwardsByUserIdAndGameId(string userId, string gameId)
        {
            return _context.MonthlyWinners
                    .Where(award => award.WinnerId == userId && award.GamePlayedId.ToLower() == gameId.ToLower())
                    .OrderBy(award => award.RecordedDate);
        }
        public IEnumerable<MonthlyWinners> GetPastMonthAwardWithIdAndGameId(string userId, string gameId)
        {
            if (gameId =="" || gameId==null || gameId.ToLower() == "overall")
            {
                return _context.MonthlyWinners
                    .Where(award => award.WinnerId == userId && award.GamePlayedId.ToLower() == "overall" && DateTime.Now.AddMonths(-1).Month == award.RecordedDate.Month);
            } else
            {
                return _context.MonthlyWinners
                    .Where(award => award.WinnerId == userId && award.GamePlayedId.ToLower() == gameId.ToLower() && DateTime.Now.AddMonths(-1).Month == award.RecordedDate.Month);
            }
        }
    }
}
