using NexusFit.Auth.API.Entities;

namespace NexusFit.Auth.API.services;

public interface ITokenService
{
    Task<string> CreateToken(ApplicationUser user);
}