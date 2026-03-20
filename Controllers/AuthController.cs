using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using elearn_server.DTO;
using elearn_server.Data;
using Microsoft.AspNetCore.Identity;
using elearn_server.Services;
using System.Security.Cryptography;

namespace elearn_server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly PasswordHasher<Models.User> _passwordHasher;

        public AuthController(AppDbContext context)
        {
            _context = context;
            _passwordHasher = new PasswordHasher<Models.User>();
        }

        [HttpGet("users")]
        public IActionResult GetUsers()
        {
            var users = _context.Users.ToList();
            return Ok(users);
        }

        private readonly IEmailService _emailService;
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _environment;

        public AuthController(AppDbContext context, IEmailService emailService, IConfiguration configuration, IWebHostEnvironment environment)
        {
            _context = context;
            _passwordHasher = new PasswordHasher<Models.User>();
            _emailService = emailService;
            _configuration = configuration;
            _environment = environment;
        }

        // Lấy ra user, để kiểm tra xem khi đăng ký có bị trùng không.
        [HttpGet("users/{id}")]
        public IActionResult GetUserById(int id)
        {
            var user = _context.Users.SingleOrDefault(u => u.UserId == id);
            if (user == null)
            {
                return NotFound(new { message = "User not found." });
            }

            return Ok(user);
        }

        // Check auth user để kiểm tra có lưu trên cookies/ session / localStorage.
        [HttpGet("check-auth")]
        public IActionResult CheckAuth()
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes("ouuiS5p6BGpIAnS+H3lgJAquyp+Smx7eVdWJXKIQIUM=");

            try
            {
                var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

                // Check if token is blacklisted
                var blacklistedToken = _context.BlacklistedTokens.SingleOrDefault(t => t.Token == token);
                if (blacklistedToken != null)
                {
                    return Unauthorized(new { message = "Token is blacklisted." });
                }

                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = "elearn-server",
                    ValidateAudience = true,
                    ValidAudience = "elearn-client",
                    ValidateLifetime = true
                };

                var principal = tokenHandler.ValidateToken(token, validationParameters, out _);
                return Ok(new
                {
                    UserId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value,
                    Name = principal.FindFirst(ClaimTypes.Name)?.Value,
                    Role = principal.FindFirst(ClaimTypes.Role)?.Value
                });
            }
            catch
            {
                return Unauthorized();
            }
        }


        // Hàm Login User
        [HttpPost("login")]
        public IActionResult Login([FromBody] UserLoginDTO loginDto)
        {
            var user = _context.Users.SingleOrDefault(u => u.Email == loginDto.Email);
            if (user == null)
            {
                return Unauthorized();
            }

            var verificationResult = _passwordHasher.VerifyHashedPassword(user, user.Password, loginDto.Password);
            if (verificationResult == PasswordVerificationResult.Failed)
            {
                return Unauthorized();
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes("ouuiS5p6BGpIAnS+H3lgJAquyp+Smx7eVdWJXKIQIUM=");
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                    new Claim(ClaimTypes.Name, user.FullName),
                    new Claim(ClaimTypes.Role, user.Role)
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                Issuer = "elearn-server",
                Audience = "elearn-client",
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return Ok(new { Token = tokenHandler.WriteToken(token) });
        }


        // api/auth/regiter
        [HttpPost("register")]
        public IActionResult Register([FromBody] UserCreateDTO registerDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingUser = _context.Users.SingleOrDefault(u => u.Email == registerDto.Email);
            if (existingUser != null)
            {
                return Conflict(new { message = "Email is already registered." });
            }

            var newUser = new Models.User
            {
                FullName = registerDto.FullName,
                Email = registerDto.Email,
                PhoneNumber = registerDto.PhoneNumber,
                Role = registerDto.Role,
                ProfilePicture = registerDto.ProfilePicture
            };

            // Hash the password before saving
            newUser.Password = _passwordHasher.HashPassword(newUser, registerDto.Password);

            _context.Users.Add(newUser);
            _context.SaveChanges();

            return CreatedAtAction(nameof(GetUserById), new { id = newUser.UserId }, newUser);
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequestDTO requestDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = _context.Users.SingleOrDefault(u => u.Email == requestDto.Email);
            if (user == null)
            {
                return Ok(new
                {
                    message = "If an account with that email exists, a password reset link has been sent."
                });
            }

            var activeTokens = _context.PasswordResetTokens
                .Where(t => t.UserId == user.UserId && !t.IsUsed && t.ExpiresAt > DateTime.UtcNow)
                .ToList();

            foreach (var token in activeTokens)
            {
                token.IsUsed = true;
                token.UsedAt = DateTime.UtcNow;
            }

            var rawToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
            var tokenHash = HashToken(rawToken);
            var expiresAt = DateTime.UtcNow.AddMinutes(15);

            var passwordResetToken = new Models.PasswordResetToken
            {
                UserId = user.UserId,
                TokenHash = tokenHash,
                ExpiresAt = expiresAt,
                IsUsed = false
            };

            _context.PasswordResetTokens.Add(passwordResetToken);
            _context.SaveChanges();

            var frontendBaseUrl = _configuration["App:FrontendBaseUrl"] ?? "http://localhost:3000";
            var resetLink = $"{frontendBaseUrl.TrimEnd('/')}/reset-password?token={Uri.EscapeDataString(rawToken)}";

            if (_emailService.IsConfigured())
            {
                await _emailService.SendPasswordResetEmailAsync(user.Email, user.FullName, resetLink);
            }

            if (_environment.IsDevelopment())
            {
                return Ok(new
                {
                    message = "Password reset request processed successfully.",
                    resetLink
                });
            }

            return Ok(new
            {
                message = "If an account with that email exists, a password reset link has been sent."
            });
        }

        [HttpPost("reset-password")]
        public IActionResult ResetPassword([FromBody] ResetPasswordRequestDTO requestDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var tokenHash = HashToken(requestDto.Token);
            var resetToken = _context.PasswordResetTokens
                .OrderByDescending(t => t.Id)
                .SingleOrDefault(t => t.TokenHash == tokenHash);

            if (resetToken == null || resetToken.IsUsed || resetToken.ExpiresAt <= DateTime.UtcNow)
            {
                return BadRequest(new { message = "Reset token is invalid or has expired." });
            }

            var user = _context.Users.SingleOrDefault(u => u.UserId == resetToken.UserId);
            if (user == null)
            {
                return NotFound(new { message = "User not found." });
            }

            user.Password = _passwordHasher.HashPassword(user, requestDto.NewPassword);
            resetToken.IsUsed = true;
            resetToken.UsedAt = DateTime.UtcNow;

            _context.SaveChanges();

            return Ok(new { message = "Password has been reset successfully." });
        }

        [HttpPost("logout")]
        public IActionResult Logout()
        {
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            if (string.IsNullOrEmpty(token))
            {
                return BadRequest(new { message = "No token provided." });
            }

            var jwtHandler = new JwtSecurityTokenHandler();
            JwtSecurityToken? jwtToken = null;
            try
            {
                jwtToken = jwtHandler.ReadJwtToken(token);
            }
            catch
            {
                return BadRequest(new { message = "Invalid token." });
            }

            var expiration = jwtToken.ValidTo;

            var blacklistedToken = new Models.BlacklistedToken
            {
                Token = token,
                Expiration = expiration
            };

            _context.BlacklistedTokens.Add(blacklistedToken);
            _context.SaveChanges();

            return Ok(new { message = "Logged out successfully." });
        }

        private static string HashToken(string rawToken)
        {
            var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(rawToken));
            return Convert.ToHexString(bytes);
        }
    }


}
