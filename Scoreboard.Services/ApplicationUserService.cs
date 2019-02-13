using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
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
        private readonly IConfiguration _config;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUpload _uploadService;
        private readonly string AzureBlobStorageConnection;

        public ApplicationUserService(IConfiguration configuration, ApplicationDbContext context, UserManager<ApplicationUser> userManager, IUpload uploadService)
        {
            _config = configuration;
            _context = context;
            _userManager = userManager;
            _uploadService = uploadService;
            AzureBlobStorageConnection = _config.GetConnectionString("AZURE_BLOB_STORAGE_USER_IMAGES");
        }

        /**
         * Returns all users in the database
         */
        public IEnumerable<ApplicationUser> GetAll()
        {
            return _context.ApplicationUsers;
        }

        /**
         * Returns all users in the database which haven't been deleted, i.e. are active
         */
        public IEnumerable<ApplicationUser> GetAllActive()
        {
            return GetAll().Where(user => user.IsProfileDeleted == false);
        }

        /**
         * Returns the user specified by the input Id
         */
        public ApplicationUser GetById(string userId)
        {
            return GetAll().FirstOrDefault(user => user.Id == userId);
        }

        /**
         * Updates a selected users motto
         */
        public async Task SetMottoAsync(string userId, string motto)
        {
            var user = GetById(userId);
            user.Motto = motto;

            _context.Update(user);
            await _context.SaveChangesAsync();
        }

        /**
         * Sets a users profile picture
         */
        public async Task SetProfileImageAsync(string userId, Uri uri)
        {
            var user = GetById(userId);
            user.ProfileImageUrl = uri.AbsoluteUri;

            _context.Update(user);
            await _context.SaveChangesAsync();
        }

        /**
         * Returns a list of users which have the input role
         */
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
            string delProfile = user.Id;
            string defaultProfileImageUrl = "/images/default-profile-image.png";

            var userRoles = await _userManager.GetRolesAsync(user);

            foreach (var role in userRoles)
            {
                await _userManager.RemoveFromRoleAsync(user, "Admin");
            }

            await DeleteUserBlobImage(user.ProfileImageUrl);

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

        /**
         * Removes the to be deleted users profile image from the blob storage to conserve storage room
         */
        private async Task DeleteUserBlobImage(string imageUrl)
        {
            var blobStorageContainer = _uploadService.GetStorageContainer(AzureBlobStorageConnection);
            // Service client is used to get the reference because we have the Uri of the image not its name within the 
            // container
            var blobImage = await blobStorageContainer.ServiceClient.GetBlobReferenceFromServerAsync(new Uri(imageUrl));
            await blobImage.DeleteAsync();
        }

    }
}
