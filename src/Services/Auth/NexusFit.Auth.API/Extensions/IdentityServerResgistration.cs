using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using NexusFit.Auth.API.Data;
using NexusFit.Auth.API.Entities;

namespace NexusFit.Auth.API.Extensions;

public static class IdentityServerResgistration
{
    public static IServiceCollection AddIdentityServices(
        this IServiceCollection services, IConfiguration configuration)
    {
            services.AddIdentityCore<ApplicationUser>(opt => 
            {
                opt.Password.RequireNonAlphanumeric = false;
                opt.Password.RequireDigit = false;
            })
            .AddRoles<ApplicationRole>()
            .AddRoleManager<RoleManager<ApplicationRole>>()
            .AddSignInManager<SignInManager<ApplicationUser>>()
            .AddRoleValidator<RoleValidator<ApplicationRole>>()
            .AddEntityFrameworkStores<IdentityContext>();

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options => 
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["TokenKey"])),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });
            services.AddAuthorization(opt => 
            {
                opt.AddPolicy("RequireAdminRole", policy => policy.RequireRole("Admin"));
            });

        return services;
    }
}
