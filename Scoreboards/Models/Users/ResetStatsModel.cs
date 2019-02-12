using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Scoreboards.Models.Users
{
    public class ResetStatsModel
    {
        public IEnumerable<DateTime> MonthNames { get; set; }
        [Required]
        public DateTime MonthSelected { get; set; }
        public string StatusMessage { get; set; }
    }
}
