using AutoMapper;
using BLRB2B.Application.Dto;
using BLRB2B.Application.Interfaces;
using BLRB2B.Domain.Entities;
using BLRB2B.Domain.Interfaces;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace BLRB2B.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly ICustomerRepository _customerRepository;
    private readonly IMapper _mapper;
    private readonly string _jwtSecret;
    private readonly string _jwtIssuer;
    private readonly string _jwtAudience;

    public AuthService(
        IUserRepository userRepository,
        ICustomerRepository customerRepository,
        IMapper mapper,
        string jwtSecret = "YourSuperSecretKeyThatIsAtLeast32CharactersLong!",
        string jwtIssuer = "BLRB2B",
        string jwtAudience = "BLRB2BClient")
    {
        _userRepository = userRepository;
        _customerRepository = customerRepository;
        _mapper = mapper;
        _jwtSecret = jwtSecret;
        _jwtIssuer = jwtIssuer;
        _jwtAudience = jwtAudience;
    }

    public async Task<AuthResponseDto> LoginAsync(LoginDto dto)
    {
        // First try to find by username, if not found try by email
        var user = await _userRepository.GetByUsernameWithCustomerAsync(dto.Username);

        if (user == null)
        {
            // Try with email (Username field might contain email)
            user = await _userRepository.GetByEmailWithCustomerAsync(dto.Username);
        }

        if (user == null || !user.IsActive)
        {
            return new AuthResponseDto
            {
                Success = false,
                Message = "Invalid username or password."
            };
        }

        var passwordValid = await VerifyPasswordAsync(dto.Password, user.PasswordHash);
        if (!passwordValid)
        {
            return new AuthResponseDto
            {
                Success = false,
                Message = "Invalid username or password."
            };
        }

        // Update last login date
        user.LastLoginDate = DateTime.UtcNow;
        await _userRepository.UpdateAsync(user);

        var token = GenerateJwtToken(user);

        return new AuthResponseDto
        {
            Success = true,
            Token = token,
            Message = "Login successful.",
            User = _mapper.Map<UserDto>(user)
        };
    }

    public async Task<AuthResponseDto> RegisterAsync(RegisterDto dto)
    {
        // Check if username exists
        if (!await IsUsernameUniqueAsync(dto.Username))
        {
            return new AuthResponseDto
            {
                Success = false,
                Message = "Username already exists."
            };
        }

        // Check if email exists
        if (!await IsEmailUniqueAsync(dto.Email))
        {
            return new AuthResponseDto
            {
                Success = false,
                Message = "Email already exists."
            };
        }

        // Validate customer if provided
        if (dto.CustomerId.HasValue)
        {
            var customer = await _customerRepository.GetByIdAsync(dto.CustomerId.Value);
            if (customer == null)
            {
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "Customer not found."
                };
            }
        }

        var user = new User
        {
            Username = dto.Username,
            Email = dto.Email,
            PasswordHash = await HashPasswordAsync(dto.Password),
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            CustomerId = dto.CustomerId,
            Role = UserRole.User,
            IsActive = true
        };

        var createdUser = await _userRepository.AddAsync(user);
        var token = GenerateJwtToken(createdUser);

        return new AuthResponseDto
        {
            Success = true,
            Token = token,
            Message = "Registration successful.",
            User = _mapper.Map<UserDto>(createdUser)
        };
    }

    public async Task<UserDto?> GetUserByIdAsync(int id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        return user == null ? null : _mapper.Map<UserDto>(user);
    }

    public async Task<UserDto?> GetUserByUsernameAsync(string username)
    {
        var user = await _userRepository.GetByUsernameAsync(username);
        return user == null ? null : _mapper.Map<UserDto>(user);
    }

    public async Task<bool> IsUsernameUniqueAsync(string username, int? excludeId = null)
    {
        return await _userRepository.IsUsernameUniqueAsync(username, excludeId);
    }

    public async Task<bool> IsEmailUniqueAsync(string email, int? excludeId = null)
    {
        return await _userRepository.IsEmailUniqueAsync(email, excludeId);
    }

    public async Task<string> HashPasswordAsync(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password);
    }

    public async Task<bool> VerifyPasswordAsync(string password, string hash)
    {
        return await Task.FromResult(BCrypt.Net.BCrypt.Verify(password, hash));
    }

    private string GenerateJwtToken(User user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_jwtSecret);

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.Username),
            new(ClaimTypes.Email, user.Email),
            new(ClaimTypes.Role, user.Role)
        };

        if (user.CustomerId.HasValue)
        {
            claims.Add(new Claim("CustomerId", user.CustomerId.Value.ToString()));
        }

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddDays(7),
            Issuer = _jwtIssuer,
            Audience = _jwtAudience,
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}
