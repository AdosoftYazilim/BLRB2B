using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BLRB2B.Application.Interfaces;
using BLRB2B.Application.Dto;

namespace BLRB2B.Web.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly IConfiguration _configuration;

    public AuthController(IAuthService authService, IConfiguration configuration)
    {
        _authService = authService;
        _configuration = configuration;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] BLRB2B.Web.Services.LoginModel model)
    {
        try
        {
            // Email'i username olarak kullan
            var dto = new LoginDto
            {
                Username = model.Email,
                Password = model.Password
            };

            var result = await _authService.LoginAsync(dto);

            if (!result.Success || result.User == null)
            {
                return Unauthorized(new { success = false, message = result.Message ?? "Invalid username or password" });
            }

            // Generate JWT Token
            var token = GenerateJwtToken(result.User);

            return Ok(new
            {
                success = true,
                token = token,
                user = result.User,
                message = "Login successful"
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        // Check if username is unique
        var isUsernameUnique = await _authService.IsUsernameUniqueAsync(dto.Username);
        if (!isUsernameUnique)
        {
            return BadRequest(new { message = "Username already exists" });
        }

        // Check if email is unique
        var isEmailUnique = await _authService.IsEmailUniqueAsync(dto.Email);
        if (!isEmailUnique)
        {
            return BadRequest(new { message = "Email already exists" });
        }

        try
        {
            var result = await _authService.RegisterAsync(dto);

            if (!result.Success || result.User == null)
            {
                return BadRequest(new { message = result.Message ?? "Registration failed" });
            }

            // Generate JWT Token
            var token = GenerateJwtToken(result.User);

            return CreatedAtAction(nameof(Login), new
            {
                success = true,
                token = token,
                user = result.User,
                message = "Registration successful"
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    private string GenerateJwtToken(UserDto user)
    {
        var securityKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_configuration["Jwt:Key"] ?? "YourVeryLongSecretKeyHereForJWTTokenGeneration123456"));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.GivenName, user.FirstName ?? string.Empty),
            new Claim(ClaimTypes.Surname, user.LastName ?? string.Empty),
            new Claim(ClaimTypes.Role, user.Role),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"] ?? "BLRB2B",
            audience: _configuration["Jwt:Audience"] ?? "BLRB2BUsers",
            claims: claims,
            expires: DateTime.Now.AddDays(7),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
