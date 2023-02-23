using IdentityModel;
using IdentityServer4;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using NexusFit.Auth.API.Dtos;
using NexusFit.Auth.API.Entities;
using NexusFit.Auth.API.Helpers;
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
    private readonly IdentityServerSettings _identityServerSettings;
    private readonly IdentityServerTools _identityServerTools;

    public AuthController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        ILogger<AuthController> logger,
        IOptions<IdentityServerSettings> identityServerOptions,
        IdentityServerTools identityServerTools)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _logger = logger;
        _identityServerSettings = identityServerOptions.Value;
        _identityServerTools = identityServerTools;
    }

    [HttpGet]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public async Task<ActionResult<UserDto>> GetCurrentUser()
    {
        var email = User.Claims.FirstOrDefault(q => q.Type == "email")?.Value;

        var user = await _userManager.Users
            .SingleOrDefaultAsync(q => q.Email == email);
        var claims = await _userManager.GetClaimsAsync(user);
        var token = await GetToken(claims.ToList());

        return new UserDto
        {
            Token = token,
            Email = user.Email
        };
    }

    [HttpPost("login")]
    public async Task<ActionResult<UserDto>> Login([FromBody] LoginDto loginDto)
    {
        var user = await _userManager.FindByEmailAsync(loginDto.Email);
        if (user == null)
            return BadRequest(new ApiResponse((int)HttpStatusCode.Unauthorized,
                "Invalid credentials."));

        if (await _userManager.CheckPasswordAsync(user, loginDto.Password) == false)
            return BadRequest(new ApiResponse((int)HttpStatusCode.Unauthorized,
                "Invalid credentials."));

        var claims = await _userManager.GetClaimsAsync(user);
        var token = await GetToken(claims.ToList());

        return new UserDto
        {
            Token = token,
            Email = user.Email
        };
    }

    [HttpPost("register")]
    public async Task<ActionResult<UserDto>> Register([FromBody] RegisterDto registerDto)
    {
        var userExists = await UserExists(registerDto.Email);
        if (userExists)
            return BadRequest(new ApiResponse((int)HttpStatusCode.BadRequest,
                "Email address already exists."));

        var user = new ApplicationUser
        {
            Email = registerDto.Email,
            UserName = registerDto.Email
        };

        var result = await _userManager.CreateAsync(user, registerDto.Password);
        if (!result.Succeeded)
        {
            _logger.LogInformation("Failed to create User", result.Errors);
            return BadRequest(new ApiResponse((int)HttpStatusCode.BadRequest,
                "Failed to create user, please contact support."));
        }

        var claims = new List<Claim>()
        {
            new Claim(JwtClaimTypes.Email, registerDto.Email),
            new Claim(JwtClaimTypes.Role, "Student"),
            new Claim(JwtClaimTypes.Role, "Admin")
        };

        result = await _userManager.AddClaimsAsync(user, claims);
        if (!result.Succeeded)
        {
            _logger.LogInformation("Failed to add claims to User", result.Errors);
            return BadRequest(new ApiResponse((int)HttpStatusCode.BadRequest,
                "Failed to create user, please contact support."));
        }

        var token = await GetToken(claims);

        return new UserDto
        {
            Token = token,
            Email = user.Email
        };
    }

    [HttpGet("userexists/{email}")]
    public async Task<bool> UserExists(string email)
    {
        return await _userManager.FindByEmailAsync(email) != null;
    }

    private async Task<string> GetToken(List<Claim> claims)
    {
        var scopes = _identityServerSettings.Scope.Split(' ');

        var token = await _identityServerTools.IssueClientJwtAsync(
            _identityServerSettings.ClientId,
            30000,
            scopes: scopes,
            additionalClaims: claims
        );

        return token;
    }
}
