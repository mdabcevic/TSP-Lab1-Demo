
using TSP_LAB1.DTOs;

namespace TSP_LAB1.Interfaces;

public interface IAuthService
{
    Task<TokenResponse?> LoginAsync(LoginRequest request, CancellationToken ct = default);

    Task<TokenResponse?> RefreshAsync(string refreshToken, CancellationToken ct = default);

    Task LogoutAsync(Guid userId, Guid sessionId, CancellationToken ct = default);

    Task LogoutAllAsync(Guid userId, CancellationToken ct = default);
}
