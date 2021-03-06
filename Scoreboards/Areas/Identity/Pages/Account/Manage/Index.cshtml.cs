﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Http.Headers;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Scoreboards.Data;
using Scoreboards.Data.Models;

namespace Scoreboards.Areas.Identity.Pages.Account.Manage
{
    public partial class IndexModel : PageModel
    {
        private readonly IConfiguration _config;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IEmailSender _emailSender;

        private readonly IUpload _uploadService;
        private readonly IApplicationUser _applicationUserService;
        private readonly string AzureBlobStorageConnection;

        public IndexModel(
            IConfiguration configuration,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IUpload uploadService,
            IApplicationUser applicationUserService,
            IEmailSender emailSender)
        {
            _config = configuration;
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _uploadService = uploadService;
            _applicationUserService = applicationUserService;
            AzureBlobStorageConnection = _config.GetConnectionString("AZURE_BLOB_STORAGE_USER_IMAGES");
        }

        public string Username { get; set; }
        public string ProfileImageUrl { get; set; }

        public bool IsEmailConfirmed { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; }

            //[Phone]
            //[Display(Name = "Phone number")]
            //public string PhoneNumber { get; set; }
            [StringLength(1000, ErrorMessage = "The {0} must be {1} max characters long.")]
            [DataType(DataType.Text)]
            [Display(Name = "Motto")]
            public string Motto { get; set; }

            [Display(Name = "Upload User Profile Image")]
            public IFormFile ImageUpload { get; set; }
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var userName = user.UserName;
            var email = user.Email;
            var motto = user.Motto;
            //var phoneNumber = await _userManager.GetPhoneNumberAsync(user);

            Username = userName;

            ProfileImageUrl = user.ProfileImageUrl;

            Input = new InputModel
            {
                Email = email,
                Motto = motto
            };

            IsEmailConfirmed = await _userManager.IsEmailConfirmedAsync(user);

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(IFormFile ImageUpload)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }


            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var email = await _userManager.GetEmailAsync(user);
            if (Input.Email != email)
            {
                var setEmailResult = await _userManager.SetEmailAsync(user, Input.Email);
                if (!setEmailResult.Succeeded)
                {
                    var userId = await _userManager.GetUserIdAsync(user);
                    throw new InvalidOperationException($"Unexpected error occurred setting email for user with ID '{userId}'.");
                }
            }

            var motto = user.Motto;
            if(Input.Motto != motto)
            {
                await _applicationUserService.SetMottoAsync(user.Id, Input.Motto);
            }

            if(ImageUpload != null)
            {
                await UploadUserProfileImage(ImageUpload);
            }

            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Your profile has been updated";

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostSendVerificationEmailAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }


            var userId = await _userManager.GetUserIdAsync(user);
            var email = await _userManager.GetEmailAsync(user);
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var callbackUrl = Url.Page(
                "/Account/ConfirmEmail",
                pageHandler: null,
                values: new { userId = userId, code = code },
                protocol: Request.Scheme);
            await _emailSender.SendEmailAsync(
                email,
                "Confirm your email at The Instillery Scoreboards",
                $"You updated your email at The Instillery Scoreboards website. Please confirm your account by  <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

            StatusMessage = "Verification email sent. Please check your email.";
            return RedirectToPage();
        }

        /*
         * Uploads User Profile image to Azure Blob storage
         */ 

        private async Task UploadUserProfileImage(IFormFile file)
        {
            var userId = _userManager.GetUserId(User);
            var container = _uploadService.GetStorageContainer(AzureBlobStorageConnection);
            // Get file name
            var contentDisposition = ContentDispositionHeaderValue.Parse(file.ContentDisposition);
            var fileName = contentDisposition.FileName.Trim('"');

            // Replace it with userId to save space in Azure blob
            // As the file with the same name will overrite the old file.
            var fileExtension = fileName.Substring(fileName.LastIndexOf('.'));
            var userIdFileName = String.Concat(userId, fileExtension);
            
            var blockBlob = container.GetBlockBlobReference(userIdFileName);

            await blockBlob.UploadFromStreamAsync(file.OpenReadStream());
            await _applicationUserService.SetProfileImageAsync(userId, blockBlob.Uri);
        }
    }
}
