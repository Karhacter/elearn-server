using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using elearn_server.DTO;
using elearn_server.Data;
using Microsoft.AspNetCore.Identity;

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

            return CreatedAtAction(nameof(GetUsers), new { id = newUser.UserId }, newUser);
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
    }
}
