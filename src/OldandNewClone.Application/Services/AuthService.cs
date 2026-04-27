using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OldandNewClone.Application.DTOs;
using OldandNewClone.Application.Interfaces;
using OldandNewClone.Application.Configuration;
using OldandNewClone.Domain.Entities;

namespace OldandNewClone.Application.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IUserRepository _userRepository;
    private readonly JwtSettings _jwtSettings;
    private readonly ILogger<AuthService> _logger;

    public AuthService(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IJwtTokenService jwtTokenService,
        IUserRepository userRepository,
        IOptions<JwtSettings> jwtSettings,
        ILogger<AuthService> logger)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _jwtTokenService = jwtTokenService;
        _userRepository = userRepository;
        _jwtSettings = jwtSettings.Value;
        _logger = logger;
    }

    public async Task<AuthResponseDto> RegisterAsync(RegisterDto registerDto)
    {
        try
        {
            // Check for existing username or email (case-insensitive) - matching Node.js
            if (await _userRepository.EmailExistsAsync(registerDto.Email.ToLower()))
            {
                _logger.LogWarning("Registration attempt with existing email: {Email}", registerDto.Email);
                return new AuthResponseDto
                {
                    Success = false,
                    ErrorMessage = "User or email already exists"
                };
            }

            if (await _userRepository.UsernameExistsAsync(registerDto.Username.ToLower()))
            {
                _logger.LogWarning("Registration attempt with existing username: {Username}", registerDto.Username);
                return new AuthResponseDto
                {
                    Success = false,
                    ErrorMessage = "User or email already exists"
                };
            }

            // Match Node.js user structure
            var user = new ApplicationUser
            {
                UserName = registerDto.Username.ToLower(), // Store lowercase like Node.js
                Email = registerDto.Email.ToLower(),
                FirstName = registerDto.FirstName,
                LastName = registerDto.LastName,
                Phone = registerDto.Phone,
                IsAdmin = registerDto.IsAdmin,
                Name = $"{registerDto.FirstName} {registerDto.LastName}",
                CreatedAt = DateTime.UtcNow,
                EmailConfirmed = true
            };

            _logger.LogInformation("Creating user: {Username} ({Email})", user.UserName, user.Email);
            var result = await _userManager.CreateAsync(user, registerDto.Password);

            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                _logger.LogWarning("User creation failed for {Email}: {Errors}", registerDto.Email, errors);
                return new AuthResponseDto
                {
                    Success = false,
                    ErrorMessage = errors
                };
            }

            _logger.LogInformation("User created successfully: {Email}", registerDto.Email);

            // Assign default role
            await _userManager.AddToRoleAsync(user, "User");

            // Generate tokens
            var accessToken = _jwtTokenService.GenerateAccessToken(user.Id, user.Email!, "User");
            var refreshToken = _jwtTokenService.GenerateRefreshToken();

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpiryDays);
            await _userRepository.UpdateAsync(user);

            _logger.LogInformation("Registration complete for user: {Email}", registerDto.Email);

            return new AuthResponseDto
            {
                Success = true,
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                ExpiresAt = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryMinutes),
                User = new UserInfoDto
                {
                    Id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Username = user.UserName!,
                    Email = user.Email!,
                    Phone = user.Phone,
                    IsAdmin = user.IsAdmin,
                    Role = user.IsAdmin ? "Admin" : "User"
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during user registration for email: {Email}", registerDto.Email);
            return new AuthResponseDto
            {
                Success = false,
                ErrorMessage = "An error occurred during registration"
            };
        }
    }

    public async Task<AuthResponseDto> LoginAsync(LoginDto loginDto)
    {
        try
        {
            // Match Node.js: Find by username or email, case-insensitive
            // Use hybrid method to support both Node.js and .NET users
            var loginInput = loginDto.UsernameOrEmail.Trim().ToLower();
            var user = await _userRepository.GetByUsernameOrEmailHybridAsync(loginInput);

            if (user == null)
            {
                _logger.LogWarning("Login attempt for non-existent user: {LoginInput}", loginInput);
                return new AuthResponseDto
                {
                    Success = false,
                    ErrorMessage = "Invalid credentials"
                };
            }

            _logger.LogInformation("Found user: {Username} ({Email}), attempting password verification", 
                user.UserName, user.Email);

            // Verify password using BCrypt
            var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);

            if (!result.Succeeded)
            {
                _logger.LogWarning("Password verification failed for user: {Username}. Result: {Result}", 
                    user.UserName, result);
                return new AuthResponseDto
                {
                    Success = false,
                    ErrorMessage = "Invalid credentials"
                };
            }

            _logger.LogInformation("Login successful for user: {Username}", user.UserName);

            // Update last login
            user.LastLoginAt = DateTime.UtcNow;

            // Get user roles
            var roles = await _userManager.GetRolesAsync(user);
            var role = roles.FirstOrDefault() ?? "User";

            // Generate tokens
            // IsAdmin flag is authoritative (covers Node.js users not in Identity roles table)
            if (user.IsAdmin) role = "Admin";

            var accessToken = _jwtTokenService.GenerateAccessToken(user.Id, user.Email!, role);
            var refreshToken = _jwtTokenService.GenerateRefreshToken();

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpiryDays);
            await _userRepository.UpdateAsync(user);

            return new AuthResponseDto
            {
                Success = true,
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                ExpiresAt = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryMinutes),
                User = new UserInfoDto
                {
                    Id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Username = user.UserName!,
                    Email = user.Email!,
                    Phone = user.Phone,
                    IsAdmin = user.IsAdmin,
                    Role = user.IsAdmin ? "Admin" : role
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during user login for input: {LoginInput}. Exception: {ExceptionType}, Message: {Message}", 
                loginDto.UsernameOrEmail, ex.GetType().Name, ex.Message);

            if (ex.InnerException != null)
            {
                _logger.LogError(ex.InnerException, "Inner exception during login");
            }

            return new AuthResponseDto
            {
                Success = false,
                ErrorMessage = $"An error occurred during login: {ex.Message}"
            };
        }
    }

    public async Task<AuthResponseDto> RefreshTokenAsync(string refreshToken)
    {
        try
        {
            var users = await Task.Run(() => _userManager.Users.ToList());
            var user = users.FirstOrDefault(u => u.RefreshToken == refreshToken);

            if (user == null || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
            {
                return new AuthResponseDto
                {
                    Success = false,
                    ErrorMessage = "Invalid or expired refresh token"
                };
            }

            var roles = await _userManager.GetRolesAsync(user);
            var role = roles.FirstOrDefault() ?? "User";

            if (user.IsAdmin) role = "Admin";
            var accessToken = _jwtTokenService.GenerateAccessToken(user.Id, user.Email!, role);
            var newRefreshToken = _jwtTokenService.GenerateRefreshToken();

            user.RefreshToken = newRefreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpiryDays);
            await _userRepository.UpdateAsync(user);

            return new AuthResponseDto
            {
                Success = true,
                AccessToken = accessToken,
                RefreshToken = newRefreshToken,
                ExpiresAt = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryMinutes),
                User = new UserInfoDto
                {
                    Id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Username = user.UserName!,
                    Email = user.Email!,
                    Phone = user.Phone,
                    IsAdmin = user.IsAdmin,
                    Role = user.IsAdmin ? "Admin" : role
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during token refresh");
            return new AuthResponseDto
            {
                Success = false,
                ErrorMessage = "An error occurred during token refresh"
            };
        }
    }

    public async Task<bool> RevokeTokenAsync(string userId)
    {
        try
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null) return false;

            user.RefreshToken = null;
            user.RefreshTokenExpiryTime = null;
            return await _userRepository.UpdateAsync(user);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during token revocation");
            return false;
        }
    }
}
