using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Scoreboards.Data;
using Scoreboards.Data.Models;
using System.Threading.Tasks;

namespace Scoreboards.Views.Shared
{
    public class _LoginModel : PageModel
    {
        private readonly IApplicationUser _applicationUserService;
        private readonly UserManager<ApplicationUser> _userManager;

        public _LoginModel(
            IApplicationUser applicationUserService,
            UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
            _applicationUserService = applicationUserService;
        }

        public string ProfileImageUrl { get; set; }

        public async Task OnGet()
        {
            var user = await _userManager.GetUserAsync(User);
            ProfileImageUrl = user.ProfileImageUrl;
        }
    }
}
