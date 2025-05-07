using Microsoft.AspNetCore.Identity;
using petmypet.Models;

namespace petmypet.Services
{
    public class SeedUserRoleInitial : ISeedUserRoleInitial
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public SeedUserRoleInitial(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public void SeedRoles()
        {
            if (!_roleManager.RoleExistsAsync("Member").Result)
            {
                IdentityRole role = new IdentityRole
                {
                    Name = "Member",
                    NormalizedName = "MEMBER"
                };
                IdentityResult roleResult = _roleManager.CreateAsync(role).Result;
            }

            if (!_roleManager.RoleExistsAsync("Admin").Result)
            {
                IdentityRole role = new IdentityRole
                {
                    Name = "Admin",
                    NormalizedName = "ADMIN"
                };
                IdentityResult roleResult = _roleManager.CreateAsync(role).Result;
            }
        }

        public void SeedUsers()
        {
            if (_userManager.FindByEmailAsync("user@user").Result == null)
            {

                ApplicationUser user = new ApplicationUser
                {
                    UserName = "user",
                    Email = "user@user",
                    Nome = "user",
                    NormalizedUserName = "USER@USER",
                    NormalizedEmail = "USER@USER",
                    EmailConfirmed = true,
                    LockoutEnabled = false,
                    SecurityStamp = Guid.NewGuid().ToString()
                };

                IdentityResult result = _userManager.CreateAsync(user, "user152535").Result;

                if (result.Succeeded)
                {
                    _userManager.AddToRoleAsync(user, "Member").Wait();
                }
            }

            if (_userManager.FindByEmailAsync("admin@admin").Result == null)
            {

                ApplicationUser user = new ApplicationUser
                {
                    UserName = "admin",
                    Nome = "admin",
                    Email = "admin@admin",
                    NormalizedUserName = "ADMIN@ADMIN",
                    NormalizedEmail = "ADMIN@ADMIN",
                    EmailConfirmed = true,
                    LockoutEnabled = false,
                    SecurityStamp = Guid.NewGuid().ToString()
                };

                IdentityResult result = _userManager.CreateAsync(user, "admin152535").Result;

                if (result.Succeeded)
                {
                    _userManager.AddToRoleAsync(user, "Admin").Wait();
                }
            }

        }

    }
}
