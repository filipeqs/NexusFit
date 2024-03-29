﻿using Microsoft.AspNetCore.Identity;

namespace NexusFit.Auth.API.Entities;

public class ApplicationUser : IdentityUser<int>
{
    public ICollection<ApplicationUserRole> UserRoles { get; set; }
}
