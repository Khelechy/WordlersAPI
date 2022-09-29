using Microsoft.AspNetCore.Identity;
using WordlersAPI.Enums;
using WordlersAPI.Models.Core;

namespace WordlersAPI.Data
{
    public static class ContextSeed
    {
        public static async Task SeedRolesAsync(UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {
            await roleManager.CreateAsync(new IdentityRole(RolesEnum.Admin.ToString()));
            await roleManager.CreateAsync(new IdentityRole(RolesEnum.User.ToString()));
        }
    }
}
