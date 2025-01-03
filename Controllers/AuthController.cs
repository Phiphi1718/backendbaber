using backend1.Data;
using backend1.Models;
using backend1.Repositories;
using backend1.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace backend1.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;
        private readonly IEmailService _emailService;

        public UserController(IUserRepository userRepository, IConfiguration configuration, IEmailService emailService)
        {
            _userRepository = userRepository;
            _configuration = configuration;
            _emailService = emailService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var user = await _userRepository.LoginAsync(request.Email, request.Password);

            if (user == null)
            {
                return Unauthorized(new { message = "Invalid email or password." });
            }

            var token = GenerateJwtToken(user);

            return Ok(new
            {
                message = "Login successful.",
                user = new
                {
                    user.FullName,
                    user.Email,
                    user.Phone
                },
                token
            });
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            try
            {
                var newUser = new User
                {
                    FullName = request.FullName,
                    Email = request.Email,
                    Phone = request.Phone,
                    PasswordHash = PasswordHelper.HashPassword(request.Password) // Hash mật khẩu
                };

                var createdUser = await _userRepository.RegisterAsync(newUser);

                return Ok(new { message = "Registration successful.", user = createdUser });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
        {
            var newPassword = await _userRepository.ForgotPasswordAsync(request.Email);
            if (newPassword == null)
            {
                return NotFound(new { message = "User not found." });
            }

            var emailSent = await _emailService.SendPasswordResetEmail(request.Email, newPassword);
            if (!emailSent)
            {
                return StatusCode(500, new { message = "Failed to send email." });
            }

            return Ok(new { message = "Password đã được gửi tới email của bạn" });
        }
    

        private string GenerateJwtToken(User user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSettings:Key"]));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("FullName", user.FullName),
                new Claim("Email", user.Email),
                new Claim("Phone", user.Phone),
                new Claim("TypeId", user.TypeId.ToString())
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["JwtSettings:Issuer"],
                audience: _configuration["JwtSettings:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(int.Parse(_configuration["JwtSettings:ExpiresInMinutes"])),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        [HttpPost("update-password")]
        public async Task<IActionResult> UpdatePassword([FromBody] UpdatePasswordRequest request)
        {
            // Kiểm tra xem mật khẩu mới và xác nhận mật khẩu mới có khớp không
            if (request.NewPassword != request.ConfirmNewPassword)
            {
                return BadRequest(new { message = "New password and confirmation do not match." });
            }

            var isUpdated = await _userRepository.UpdatePasswordAsync(request.Email, request.OldPassword, request.NewPassword);
            if (!isUpdated)
            {
                return BadRequest(new { message = "Invalid old password or failed to update password." });
            }

            return Ok(new { message = "Password updated successfully." });
        }

    }

    public class LoginRequest
    {
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
    }

    public class RegisterRequest
    {
        public string FullName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public string Password { get; set; } = null!;
    }

    public class ForgotPasswordRequest
    {
        public string Email { get; set; } = null!;
    }
}
