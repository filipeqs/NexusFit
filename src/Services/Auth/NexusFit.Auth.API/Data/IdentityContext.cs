using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NexusFit.Auth.API.Entities;

namespace NexusFit.Auth.API.Data;

public class IdentityContext : IdentityDbContext<
        ApplicationUser,
        ApplicationRole, 
        int, 
        IdentityUserClaim<int>, 
        ApplicationUserRole, 
        IdentityUserLogin<int>, 
        IdentityRoleClaim<int>,
        IdentityUserToken<int>>
{
    public IdentityContext(DbContextOptions<IdentityContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<ApplicationUser>()
            .HasMany(q => q.UserRoles)
            .WithOne(q => q.User)
            .HasForeignKey(q => q.UserId)
            .IsRequired();

        builder.Entity<ApplicationRole>()
            .HasMany(q => q.UserRoles)
            .WithOne(q => q.Role)
            .HasForeignKey(q => q.RoleId)
            .IsRequired();
    }
}
