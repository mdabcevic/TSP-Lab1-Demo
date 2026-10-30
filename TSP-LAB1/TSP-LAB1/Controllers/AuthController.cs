
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TSP_LAB1.DTOs;
using TSP_LAB1.Interfaces;

namespace TSP_LAB1.Controllers;

[ApiController]
[Route("api/auth")]
public sealed class AuthController(IAuthService authService) : ControllerBase
{
    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<ActionResult<TokenResponse>> Login(LoginRequest request, CancellationToken ct)
    {
        var result = await authService.LoginAsync(request, ct);

        return result is null
            ? Unauthorized(new { message = "Invalid credentials." })
            : Ok(result);
    }


    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout(CancellationToken ct)
    {
        var identity = GetCurrentIdentity();

        if (identity is null)
            return Unauthorized();

        await authService.LogoutAsync(identity.Value.UserId, identity.Value.SessionId, ct);
        return NoContent();
    }

    [AllowAnonymous]
    [HttpPost("refresh")]
    public async Task<ActionResult<TokenResponse>> Refresh(RefreshTokenRequest request, CancellationToken ct)
    {
        var result = await authService.RefreshAsync(request.RefreshToken, ct);

        return result is null
            ? Unauthorized(new { message = "Invalid refresh token." })
            : Ok(result);
    }

    [Authorize]
    [HttpPost("logout-all")]
    public async Task<IActionResult> LogoutAll(CancellationToken ct)
    {
        var identity = GetCurrentIdentity();

        if (identity is null)
            return Unauthorized();

        await authService.LogoutAllAsync(identity.Value.UserId, ct);
        return NoContent();
    }

    private (Guid UserId, Guid SessionId)? GetCurrentIdentity()
    {
        var sub = User.FindFirstValue("sub");
        var sid = User.FindFirstValue("sid");

        if (!Guid.TryParse(sub, out var userId) || !Guid.TryParse(sid, out var sessionId))
            return null;

        return (userId, sessionId);
    }
}
