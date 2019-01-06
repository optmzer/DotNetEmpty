using Microsoft.AspNetCore.Identity;
using System;

namespace Scoreboards.Data.Models
{
    public class ApplicationRole : IdentityRole
    {
        public ApplicationRole() : base() {}

        public ApplicationRole(string roleName) : base(roleName) { }

        public ApplicationRole(
              string roleName
            , string description) : base(roleName)
        {
            this.Description = description;
        }

        public string Description { get; set; } = "Was Not Provided";
        public DateTime RoleCreatinonDate { get; set; } = DateTime.Now;
    }
}
