using Scoreboards.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Scoreboards.Data
{
    public interface IMonthlyWinners
    {
        IEnumerable<MonthlyWinners> GetAll();
        IEnumerable<MonthlyWinners> GetAllAwardsByUserId(string userId);
        IEnumerable<MonthlyWinners> GetAllAwardsByUserIdAndGameId(string userId, string gameId);
        IEnumerable<MonthlyWinners> GetPastMonthAwardWithIdAndGameId(string userId, string gameId);
        IEnumerable<MonthlyWinners> GetPastMonthAwardWithIdAndGameId(IEnumerable<MonthlyWinners> monthlyWinners, string userId, string gameId);
        List<string> GetAllMonths();
    }
}
