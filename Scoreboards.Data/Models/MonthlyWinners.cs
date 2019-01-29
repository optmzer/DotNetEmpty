using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Scoreboards.Data.Models
{
    public class MonthlyWinners
    {
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }
        // "OVERALL" For identifying the winner for all games combined
        [Required]
        public string GamePlayedId { get; set; }
        [Required]
        public string WinnerId { get; set; }
        [Required]
        public DateTime RecordedDate { get; set; } = DateTime.Now;
    }
}