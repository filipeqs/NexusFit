using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NexusFit.Auth.API.Data;
using NexusFit.Auth.API.Entities;

namespace NexusFit.Auth.API.Extensions;

public static class IdentityServerResgistration
{
    public static IServiceCollection AddIdentityServices(
        this IServiceCollection services, IConfiguration configuration,
        IWebHostEnvironment environment)
    {
        services.AddDbContext<IdentityContext>(options =>
        {
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
        });

        services
            .AddIdentity<ApplicationUser, IdentityRole>()
            .AddEntityFrameworkStores<IdentityContext>()
            .AddSignInManager<SignInManager<ApplicationUser>>()
            .AddDefaultTokenProviders();

        var identityServer = services.AddIdentityServer(options =>
        {
            options.Events.RaiseErrorEvents = true;
            options.Events.RaiseInformationEvents = true;
            options.Events.RaiseFailureEvents = true;
            options.Events.RaiseSuccessEvents = true;

            // see https://identityserver4.readthedocs.io/en/latest/topics/resources.html
            options.EmitStaticAudienceClaim = true;
        })
            .AddInMemoryIdentityResources(IdentityConfiguration.IdentityResources)
            .AddInMemoryApiResources(IdentityConfiguration.ApiResources)
            .AddInMemoryApiScopes(IdentityConfiguration.ApiScopes)
            .AddInMemoryClients(IdentityConfiguration.Clients)
            .AddAspNetIdentity<ApplicationUser>();

        identityServer.AddDeveloperSigningCredential();

        return services;
    }
}
