using Microsoft.AspNetCore.Identity;

namespace NexusFit.Auth.API.Entities;

public class ApplicationRole : IdentityRole<int>
{
    public ICollection<ApplicationUserRole> UserRoles { get; set; }
}