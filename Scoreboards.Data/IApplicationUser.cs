using Scoreboards.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Scoreboards.Data
{
    public interface IApplicationUser
    {
        ApplicationUser GetById(string userId);
        IEnumerable<ApplicationUser> GetAll();
        Task SetProfileImageAsync(string userId, Uri uri);
        Task SetMottoAsync(string userId, string motto);
        Task SetRating(string userId, Type type);
        IEnumerable<ApplicationUser> GetByRole(string userRole);
    }
}
