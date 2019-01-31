using Microsoft.AspNetCore.Identity;
using System;

namespace Scoreboards.Data.Models
{
    public class ApplicationUser : IdentityUser
    {
        public int Rating { get; set; } = 0;
        public string ProfileImageUrl { get; set; } = "/images/default-profile-image.png";
        public DateTime MemberSince { get; set; } = DateTime.Now;
        // When profile is deleted set to true.
        public bool IsProfileDeleted { get; set; } = false;
        public string Motto { get; set; } = "Game the life";

        //For future use
        public virtual Office UsersOffice { get; set; }
    }
}
