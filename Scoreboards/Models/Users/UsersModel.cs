using Scoreboards.Data.Models;
using System.Collections.Generic;

namespace Scoreboards.Models.Users
{
    public class UsersModel
    {
        public IEnumerable<ApplicationUser> AppUsers { get; set; }
        public IEnumerable<ApplicationUser> ListOfAdmins { get; set; }
    }
}
