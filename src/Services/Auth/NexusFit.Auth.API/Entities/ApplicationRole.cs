using Microsoft.AspNetCore.Identity;

namespace NexusFit.Auth.API.Entities;

public class ApplicationRole : IdentityRole<int>
{
    public ApplicationRole()
    {
    }
    
    public ApplicationRole(string role) : base(role)
    {
    }

    public ICollection<ApplicationUserRole> UserRoles { get; set; }
}