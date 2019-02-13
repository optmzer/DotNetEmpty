using Scoreboards.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Scoreboards.Data
{
    public interface IMonthlyWinners
    {
        IEnumerable<MonthlyWinners> GetAll();
        IEnumerable<MonthlyWinners> GetAllAwardsByUserId(string userId);
        IEnumerable<MonthlyWinners> GetPastMonthAwardWithIdAndGameId(string userId, string gameId);
        IEnumerable<MonthlyWinners> GetPastMonthAwardWithIdAndGameId(string userId, string gameId, DateTime monthFetched);
        IEnumerable<MonthlyWinners> GetPastMonthAwardWithIdAndGameId(IEnumerable<MonthlyWinners> monthlyWinners, string userId, string gameId);
        IEnumerable<MonthlyWinners> GetPastMonthAwardWithIdAndGameId(IEnumerable<MonthlyWinners> monthlyWinners, string userId, string gameId, DateTime monthFetched);
        List<string> GetAllMonths();
        string GetPastMonthWinnerWithGameId(string gameId);
        Task AddNewWinnerAsync(string gameId);
    }
}
