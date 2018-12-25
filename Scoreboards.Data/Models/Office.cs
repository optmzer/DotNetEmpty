using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Scoreboards.Data.Models
{
    public class Office
    {
        public int Id { get; set; }

        [Required]
        [StringLength(30, ErrorMessage = "Limit office name to 30 characters.")]
        public string Name { get; set; }

        public string Address { get; set; }

        [Required]
        public string Telephone { get; set; }
        public string Description { get; set; }
        public DateTime OpenDate { get; set; }
        public string ImageUrl { get; set; }

        public virtual IEnumerable<ApplicationUser> ListOfStaff { get; set; }
    }
}
