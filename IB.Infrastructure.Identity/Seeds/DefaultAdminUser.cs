using IB.Core.Application.Enums;
using IB.Infrastructure.Identity.Entities;
using Microsoft.AspNetCore.Identity;

namespace IB.Infrastructure.Identity.Seeds
{
    public static class DefaultAdminUser
    {
        public static async Task SeedAsync(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            ApplicationUser defaultUser = new();
            defaultUser.UserName = "adminuser";
            defaultUser.Email = "adminuser@gmail.com";
            defaultUser.FirstName = "Maui";
            defaultUser.LastName = "Jaquez";
            defaultUser.IdNumber = "733123456";
            defaultUser.PhoneNumber = "8096577385";
            defaultUser.IsActive = true;
            defaultUser.EmailConfirmed = true;
            defaultUser.PhoneNumberConfirmed = true;

            if (userManager.Users.All(u => u.Id != defaultUser.Id))
            {
                var user = await userManager.FindByEmailAsync(defaultUser.Email);
                if (user == null)
                {
                    await userManager.CreateAsync(defaultUser, "123Pa$$word!");
                    await userManager.AddToRoleAsync(defaultUser, Roles.Admin.ToString());
                }
            }

        }
    }
}
