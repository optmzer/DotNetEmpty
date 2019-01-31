using Microsoft.AspNetCore.Identity;
using Scoreboards.Data;
using Scoreboards.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

/*
 * Provides functionality to updat user data
 */ 

namespace Scoreboards.Services
{
    public class ApplicationUserService : IApplicationUser
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ApplicationUserService(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IEnumerable<ApplicationUser> GetAll()
        {
            return _context.ApplicationUsers;
        }

        public IEnumerable<ApplicationUser> GetAllActive()
        {
            return GetAll().Where(user => user.IsProfileDeleted == false);
        }

        public ApplicationUser GetById(string userId)
        {
            return GetAll().FirstOrDefault(user => user.Id == userId);
        }

        public Task SetRating(string userId, Type type)
        {
            throw new NotImplementedException();
        }

        public async Task SetMottoAsync(string userId, string motto)
        {
            var user = GetById(userId);
            user.Motto = motto;

            _context.Update(user);
            await _context.SaveChangesAsync();
        }

        public async Task SetProfileImageAsync(string userId, Uri uri)
        {
            var user = GetById(userId);
            user.ProfileImageUrl = uri.AbsoluteUri;

            _context.Update(user);
            await _context.SaveChangesAsync();
        }

        public IEnumerable<ApplicationUser> GetByRole(string userRole)
        {
            return
                from user in GetAll()
                join u_roles in _context.UserRoles on user.Id equals u_roles.UserId
                join roles in _context.Roles on u_roles.RoleId equals roles.Id
                where roles.Name == userRole
                select user;
        }

        /**
         * This does not delete profile but replaces user data with
         * "Deleted user instead of the name"
         * UserName
         * NormalizedUserName
         * Email
         * NormalizedEmail
         * EmailConfirmed
         * ProfileImageUrl = /images/default-profile-image.png
         * Motto
         */

        public async Task DeleteUserProfileAsync(ApplicationUser user)
        {
            string delProfile = "Profile Deleted";
            string defaultProfileImageUrl = "/images/default-profile-image.png";

            var userRoles = await _userManager.GetRolesAsync(user);

            foreach (var role in userRoles)
            {
                await _userManager.RemoveFromRoleAsync(user, "Admin");
            }

            user.UserName = delProfile;
            user.NormalizedUserName = delProfile.Normalize();
            user.Email = delProfile;
            user.NormalizedEmail = delProfile.Normalize();
            user.EmailConfirmed = false;
            user.ProfileImageUrl = defaultProfileImageUrl;
            user.Motto = delProfile;
            user.IsProfileDeleted = true;

            _context.Update(user);
            await _context.SaveChangesAsync();
        }

    }
}
