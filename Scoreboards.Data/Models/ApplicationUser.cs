using Microsoft.AspNetCore.Identity;
using System;

namespace Scoreboards.Data.Models
{
    public class ApplicationUser : IdentityUser
    {
        public int Rating { get; set; } = 0;
        public string ProfileImageUrl { get; set; } = "/images/scoreboards/default_avatar.png";
        public DateTime MemberSince { get; set; } = DateTime.Now;
        //public bool IsActive { get; set; } = true;
        public string Motto { get; set; } = "Game the life";
        
        //For future use
        public virtual Office UsersOffice { get; set; }
    }
}
