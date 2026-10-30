
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using TSP_LAB1.Data;
using TSP_LAB1.DTOs;
using TSP_LAB1.Interfaces;
using TSP_LAB1.Models;
using TSP_LAB1.Security;

namespace TSP_LAB1.Services;

public sealed class AuthService(
    IUserRepository users,
    //IPasswordHasher<AppUser> passwordHasher,
    ITokenService tokenService,
    IOptions<JwtOptions> options,
    ShopDbContext db) : IAuthService
{
    private readonly JwtOptions _jwt = options.Value; // Pristup JWT postavkama za kreiranje sjednice.

    public async Task<TokenResponse?> LoginAsync(LoginRequest request, CancellationToken ct = default)
    {
        var email = request.Email.Trim();

        // 1. Dohvat korisnika
        var user = await users.GetByEmailAsync(email, ct);

        // 2. TODO: Provjera postoji li korisnik

        // 3. TODO: Odgovara li lozinka korisniku (plain text equality provjera je dovoljna)

        // 4. TODO: Provjera statusa korisničkog računa

        // -- Ako je korisnik uspjesno identificiran, nastavljamo s kreiranjem tokena i sesija.

        // 5. Generiranje identifikatora sesije
        var sessionId = Guid.NewGuid();

        // 6. Generiranje refresh tokena
        var refreshToken = "refreshToken"; // TODO: Pronadjite vec pripremljenu metode za stvaranje tokena i napravite novi refresh token

        var now = DateTimeOffset.UtcNow;

        var session = new UserSession
        {
            Id = sessionId,
            UserId = user.Id,
            CurrentRefreshTokenHash = "refreshToken", // TODO: Dodijelite kreirani refresh token sjednici koju kreirate.
            CreatedAt = now,
            ExpiresAt = now.AddDays(_jwt.RefreshTokenDays)
        };

        // 7. Spremanje sjednice u bazu
        db.UserSessions.Add(session);
        await db.SaveChangesAsync(ct);

        // 8. Izdavanje JWT access tokena
        var accessToken = "JWT sa ID korisnika i ID sjednice"; // TODO: Pronadjite vec pripremljenu metode za stvaranje tokena i napravite novi Access token na temelju korisnika i ID sjednice

        // 9. Korisniku proslijerdite tokene koje će dalje upotrebljavati za autentifikaciju.
        return new TokenResponse(
            AccessToken: accessToken,
            RefreshToken: refreshToken,
            TokenType: "Bearer",
            ExpiresAt: DateTimeOffset.UtcNow.AddMinutes(_jwt.AccessTokenMinutes));
    }

    /// <summary>
    /// Invalidates the current session for authenticated user.
    /// </summary>
    /// <param name="userId">ID of the user attempting the logout.</param>
    /// <param name="sessionId">ID of the session to be invalidated.</param>
    /// <param name="ct"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public async Task LogoutAsync(Guid userId, Guid sessionId, CancellationToken ct = default)
    {
        // TODO: pronađite odgovarajuću aktivnu sesiju te azurirajte relevantne Revoked stupce s vremenskom oznakom i razlogom invalidacije.
        throw new NotImplementedException();
    }

    public async Task LogoutAllAsync(Guid userId, CancellationToken ct = default)
    {
        var now = DateTimeOffset.UtcNow;
        await db.UserSessions
            .Where(s => s.UserId == userId && s.RevokedAt == null)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(s => s.RevokedAt, now)
                .SetProperty(s => s.RevocationReason, "logout_all"),
                ct);
    }

    public async Task<TokenResponse?> RefreshAsync(string refreshToken, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(refreshToken) || refreshToken.Length > 512)
            return null;

        var hash = tokenService.HashRefreshToken(refreshToken);
        var now = DateTimeOffset.UtcNow;

        // 1. Pronađi sesiju s trenutačno aktivnim tokenom.
        var session = await db.UserSessions
            .AsNoTracking()
            .Include(s => s.User)
            .SingleOrDefaultAsync(s => s.CurrentRefreshTokenHash == hash, ct);

        if (session is null)
        {
            // Token možda pripada povijesti već iskorištenih.
            await RevokeOnReuseAsync(hash, ct);
            return null;
        }

        // 2. Provjeri može li sesija još izdavati tokene.
        if (session.RevokedAt is not null || session.ExpiresAt <= now || !session.User.IsActive)
            return null;

        // 3. Pripremi novi refresh token.
        var newRefreshToken = tokenService.GenerateRefreshToken();
        var newHash = tokenService.HashRefreshToken(newRefreshToken);

        // 4. Promjena aktivnog tokena i upis u povijest i moraju se izvršiti u jednoj transakciji.
        await using var transaction = await db.Database.BeginTransactionAsync(ct);

        // Uvjet nad starim hashom služi kao optimistic concurrency zaštita.
        var affected = await db.UserSessions
            .Where(s =>
                s.Id == session.Id &&
                s.CurrentRefreshTokenHash == hash &&
                s.RevokedAt == null &&
                s.ExpiresAt > now)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(s => s.CurrentRefreshTokenHash, newHash)
                .SetProperty(s => s.LastRotatedAt, now),
                ct);

        if (affected != 1)
        {
            await transaction.RollbackAsync(ct);
            // Moguće je da je drugi zahtjev upravo iskoristio isti refresh token.
            await RevokeOnReuseAsync(hash, ct);
            return null;
        }

        // 5. Stari token više nije aktivan, ali čuvamo njegov hash radi detekcije reusea.
        db.UsedRefreshTokens.Add(new UsedRefreshToken
        {
            SessionId = session.Id,
            TokenHash = hash,
            UsedAt = now
        });

        await db.SaveChangesAsync(ct);
        await transaction.CommitAsync(ct);

        // 6. Izdaj novi JWT vezan uz ISTU sesiju.
        var accessToken = tokenService.CreateAccessToken(session.User, session.Id);

        return new TokenResponse(
            AccessToken: accessToken,
            RefreshToken: newRefreshToken,
            TokenType: "Bearer",
            ExpiresAt: DateTimeOffset.UtcNow.AddMinutes(_jwt.AccessTokenMinutes));
    }

    private async Task RevokeOnReuseAsync(string tokenHash, CancellationToken ct)
    {
        var used = await db.UsedRefreshTokens
            .AsNoTracking()
            .SingleOrDefaultAsync(t => t.TokenHash == tokenHash, ct);

        if (used is null)
            return;

        var now = DateTimeOffset.UtcNow;

        await db.UserSessions
            .Where(s =>
                s.Id == used.SessionId &&
                s.RevokedAt == null)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(s => s.RevokedAt, now)
                .SetProperty(s => s.RevocationReason, "refresh_token_reuse"),
                ct);
    }
}
