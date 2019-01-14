using Scoreboards.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Scoreboards.Models.Users
{
    public class UsersModel
    {
        public IEnumerable<ApplicationUser> AppUsers { get; set; }
    }
}
