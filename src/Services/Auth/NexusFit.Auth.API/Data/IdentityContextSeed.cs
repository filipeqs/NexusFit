using IdentityModel;
using Microsoft.AspNetCore.Identity;
using NexusFit.Auth.API.Entities;
using System.Security.Claims;

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
                if (await userManager.FindByEmailAsync("filipe.silva@email.com") == null)
                {
                    var user = new ApplicationUser
                    {
                        Email = "filipe.silva@email.com",
                        UserName = "filipe.silva@email.com"
                    };

                    await userManager.CreateAsync(user, "P@ssw0rd");

                    await userManager.AddClaimsAsync(user, new List<Claim>()
                    {
                        new Claim(JwtClaimTypes.Name, $"Filipe Silva"),
                        new Claim(JwtClaimTypes.Role, "Student"),
                        new Claim(JwtClaimTypes.Role, "Admin")
                    });
                }
            }
        }
    }
}
