using System.Security.Claims;
using elearn_server.Application.Responses;
using elearn_server.Application.DTOs;
using elearn_server.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using elearn_server.Application.Common;
namespace elearn_server.Presentation.Controllers;

[Route("api/[controller]")]
public class AuthController(IAuthService authService) : ApiControllerBase
{
    private const string AuthCookieName = "elearn_auth_token";

    [HttpGet("users")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetUsers() => FromResult(await authService.GetUsersAsync());

    [HttpGet("users/{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetUserById(int id) => FromResult(await authService.GetUserByIdAsync(id));

    [Authorize]
    [HttpGet("check-auth")]
    public async Task<IActionResult> CheckAuth() => FromResult(await authService.CheckAuthAsync(GetTokenFromRequest(), User));

    [HttpPost("login")]
    [EnableRateLimiting("auth-login")]
    public async Task<IActionResult> Login([FromBody] UserLoginDTO loginDto)
    {
        var result = await authService.LoginAsync(loginDto, HttpContext.Connection.RemoteIpAddress?.ToString(), Request.Headers.UserAgent.ToString());
        if (result.Success && result.Data is LoginResponse data)
        {
            AppendAuthCookie(data.AccessToken, data.ExpiresAtUtc);
        }

        return FromResult(result);
    }

    // update lại register để đăng ký dưới dạng user, không phải admin tạo user

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterUserDTO registerDto) =>
        FromResult(await authService.RegisterAsync(registerDto, Request.Scheme + "://" + Request.Host.Value));

    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequestDTO request)
    {
        var result = await authService.RefreshTokenAsync(request, HttpContext.Connection.RemoteIpAddress?.ToString(), Request.Headers.UserAgent.ToString());
        if (result.Success && result.Data is LoginResponse data)
        {
            AppendAuthCookie(data.AccessToken, data.ExpiresAtUtc);
        }

        return FromResult(result);
    }

    [HttpPost("verify-email")]
    public async Task<IActionResult> VerifyEmail([FromBody] VerifyEmailRequestDTO request) => FromResult(await authService.VerifyEmailAsync(request));

    [HttpPost("forgot-password")]
    [EnableRateLimiting("auth-forgot-password")]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequestDTO requestDto) => FromResult(await authService.ForgotPasswordAsync(requestDto));

    [HttpPost("reset-password")]
    [EnableRateLimiting("auth-reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequestDTO requestDto) => FromResult(await authService.ResetPasswordAsync(requestDto));

    [Authorize]
    [HttpPost("change-password")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequestDTO requestDto)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!int.TryParse(userIdClaim, out var userId))
        {
            return FromResult(ServiceResult<object>.Fail(StatusCodes.Status401Unauthorized, "Invalid user session."));
        }

        return FromResult(await authService.ChangePasswordAsync(userId, requestDto, HttpContext.Connection.RemoteIpAddress?.ToString(), Request.Headers.UserAgent.ToString()));
    }

    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        var result = await authService.LogoutAsync(GetTokenFromRequest(), HttpContext.Connection.RemoteIpAddress?.ToString(), Request.Headers.UserAgent.ToString());
        if (result.Success)
        {
            Response.Cookies.Delete(AuthCookieName, BuildCookieOptions(DateTimeOffset.UtcNow));
        }

        return FromResult(result);
    }

    private string GetTokenFromRequest()
    {
        var authorizationHeader = Request.Headers["Authorization"].ToString();
        if (!string.IsNullOrWhiteSpace(authorizationHeader) && authorizationHeader.StartsWith("Bearer "))
        {
            return authorizationHeader["Bearer ".Length..].Trim();
        }

        return Request.Cookies.TryGetValue(AuthCookieName, out var cookieToken) ? cookieToken : string.Empty;
    }

    private void AppendAuthCookie(string token, DateTime expiresAtUtc)
    {
        Response.Cookies.Append(AuthCookieName, token, BuildCookieOptions(new DateTimeOffset(expiresAtUtc)));
    }

    private CookieOptions BuildCookieOptions(DateTimeOffset expiresAtUtc) => new()
    {
        HttpOnly = true,
        Secure = Request.IsHttps,
        SameSite = SameSiteMode.Strict,
        Expires = expiresAtUtc,
        IsEssential = true,
        Path = "/"
    };
}

