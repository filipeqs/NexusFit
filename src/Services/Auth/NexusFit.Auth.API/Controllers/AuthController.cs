using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NexusFit.Auth.API.Dtos;
using NexusFit.Auth.API.Entities;
using NexusFit.Auth.API.services;
using NexusFit.BuildingBlocks.Common.Models;
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
    private readonly ITokenService _tokenService;
    private readonly IMapper _mapper;

    public AuthController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        ILogger<AuthController> logger,
        ITokenService tokenService,
        IMapper mapper)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _logger = logger;
        _tokenService = tokenService;
        _mapper = mapper;
    }

    [HttpGet]
    [Authorize]
    public async Task<ActionResult<UserDto>> GetCurrentUser()
    {
        var email = User.Claims.FirstOrDefault(q => q.Type == ClaimTypes.Email)?.Value;

        var user = await _userManager.Users
            .SingleOrDefaultAsync(q => q.Email == email);

        return new UserDto
        {
            Token = await _tokenService.CreateToken(user),
            Email = user.Email
        };
    }

    [HttpPost("login")]
    public async Task<ActionResult<UserDto>> Login([FromBody] LoginDto loginDto)
    {
        var user = await _userManager.FindByEmailAsync(loginDto.Email);
        if (user is null)
            return Unauthorized(new ApiResponse((int)HttpStatusCode.Unauthorized,
                "Invalid credentials."));

        var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);
        if (result.Succeeded is false)
            return Unauthorized(new ApiResponse((int)HttpStatusCode.Unauthorized,
                "Invalid credentials."));

        return new UserDto
        {
            Token = await _tokenService.CreateToken(user),
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

        var user = _mapper.Map<ApplicationUser>(registerDto);

        var result = await _userManager.CreateAsync(user, registerDto.Password);
        if (!result.Succeeded)
        {
            _logger.LogInformation("Failed to create User", result.Errors);
            return BadRequest(new ApiResponse((int)HttpStatusCode.BadRequest,
                "Failed to create user, please contact support."));
        }

        var roleResult = await _userManager.AddToRoleAsync(user, "Student");
        if (roleResult.Succeeded is false)
        {
            _logger.LogInformation("Failed to add role", result.Errors);
            return BadRequest(new ApiResponse((int)HttpStatusCode.BadRequest,
                "Failed to create user, please contact support."));
        }

        return new UserDto
        {
            Token = await _tokenService.CreateToken(user),
            Email = user.Email
        };
    }

    [HttpGet("userexists/{email}")]
    public async Task<bool> UserExists(string email)
    {
        return await _userManager.FindByEmailAsync(email) != null;
    }
}
