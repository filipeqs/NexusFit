using IdentityModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NexusFit.Auth.API.Dtos;
using NexusFit.Auth.API.Entities;
using NexusFit.BuildingBlocks.ExceptionHandling.Models;
using System.Net;
using System.Security.Claims;

namespace NexusFit.Auth.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    public readonly SignInManager<ApplicationUser> _signInManager;
    private ILogger<AuthController> _logger;

    public AuthController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        ILogger<AuthController> logger)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _logger = logger;
    }

    [HttpPost]
    public async Task<ActionResult<UserDto>> Register([FromBody] RegisterDto registerDto)
    {
        if (await _userManager.FindByEmailAsync(registerDto.Email) != null)
            return BadRequest(new ApiResponse((int)HttpStatusCode.BadRequest, "Email address already exists."));

        var user = new ApplicationUser
        {
            Email = registerDto.Email,
            UserName = registerDto.Email
        };

        var result = await _userManager.CreateAsync(user, registerDto.Password);
        if (!result.Succeeded)
        {
            _logger.LogInformation("Failed to create User", result.Errors);
            return BadRequest(new ApiResponse((int)HttpStatusCode.BadRequest, "Failed to create user, please contact support."));
        }

        result = await _userManager.AddClaimsAsync(user, new List<Claim>()
            {
                new Claim(JwtClaimTypes.Name, $"{registerDto.FirstName} {registerDto.LastName}"),
                new Claim(JwtClaimTypes.Role, "Student"),
                new Claim(JwtClaimTypes.Role, "Admin")
            });
        if (!result.Succeeded)
        {
            _logger.LogInformation("Failed to add claims to User", result.Errors);
            return BadRequest(new ApiResponse((int)HttpStatusCode.BadRequest, "Failed to create user, please contact support."));
        }

        return new UserDto() { Email = user.Email };
    }
}
