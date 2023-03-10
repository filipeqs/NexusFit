using Microsoft.AspNetCore.Identity;
using NexusFit.Auth.API.Entities;

namespace NexusFit.Auth.API.Data
{
    public class IdentityContextSeed
    {
        public async Task SeedAsync(
            SignInManager<ApplicationUser> signInManager, 
            UserManager<ApplicationUser> userManager, 
            IHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                if (await userManager.FindByEmailAsync("admin.user@email.com") == null)
                {
                    var user = new ApplicationUser
                    {
                        Email = "admin.user@email.com",
                        UserName = "admin.user@email.com"
                    };

                    await userManager.CreateAsync(user, "P@ssw0rd");
                    await userManager.AddToRoleAsync(user, "Admin");
                }
            }
        }
    }
}
