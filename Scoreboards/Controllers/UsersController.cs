using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Scoreboards.Data;
using Scoreboards.Data.Models;
using Scoreboards.Models.Users;

namespace Scoreboards.Controllers
{
    public class UsersController : Controller
    {
        private readonly IApplicationUser _userService;

        public UsersController(IApplicationUser userService)
        {
            _userService = userService;
        }

        public IActionResult Index(string SearchInput)
        {
            var model = new UsersModel
            {
                AppUsers = _userService.GetAll()
            };
            // Display to the page
            return View(model);
        }
    }
}