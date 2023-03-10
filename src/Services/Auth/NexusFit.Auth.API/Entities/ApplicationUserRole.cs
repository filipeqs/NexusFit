using Microsoft.AspNetCore.Identity;

namespace NexusFit.Auth.API.Entities;

public class ApplicationUserRole : IdentityUserRole<int>
{
    public ApplicationUser User { get; set; }
    public ApplicationRole Role { get; set; }
}