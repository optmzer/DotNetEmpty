using System;
using System.Collections.Generic;
using System.Text;

namespace Scoreboards.Data.Models
{
    public class MonthlyWinners
    {
        public int Id { get; set; }
        public string Title { get; set; }
        // "OVERALL" For identifying the winner for all games combined
        public string GamePlayedId { get; set; }
        public string WinnerId { get; set; }
        public DateTime RecordedDate { get; set; } = DateTime.Now;
    }
}