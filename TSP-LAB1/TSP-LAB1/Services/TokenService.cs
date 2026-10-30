
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using TSP_LAB1.Interfaces;
using TSP_LAB1.Models;
using TSP_LAB1.Security;

namespace TSP_LAB1.Services;

/// <summary>
/// Service for issuing all the relevant security Tokens.
/// </summary>
/// <param name="options">JWT configuration for the application.</param>
public sealed class TokenService(IOptions<JwtOptions> options) : ITokenService
{
    private readonly JwtOptions _options = options.Value;

    /// <summary>
    /// Creates the string representation of valid JWT based on settings and provided user context.
    /// </summary>
    /// <param name="user">Author of the request whose credentials have been validated.</param>
    /// <param name="sessionId">Created unique ID for session to be included in token claims.</param>
    /// <returns></returns>
    public string CreateAccessToken(AppUser user, Guid sessionId)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.SigningKey)); // Ključ za potpisivanje
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256); // Algoritam za potpisivanje
        var now = DateTime.UtcNow;

        // Popis tvrdnji (claims) koje opisuju korisnika kojem izdajemo JWT vjerodajnicu.
        var claims = new List<Claim>
        {
            //TODO: nadopisati odgovarajuce tvrdnje za identitet, sesiju i rolu korisnika.
            new("av", user.AuthVersion.ToString()),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        // Kreiranje validnog JWT tokena s odgovarajucom konfiguracijom, korisnickim tvrdnjama i potpisujemo ga
        var jwt = new JwtSecurityToken(
            issuer: _options.Issuer,
            audience: _options.Audience,
            claims: claims,
            notBefore: now,
            expires: now.AddMinutes(_options.AccessTokenMinutes),
            signingCredentials: credentials);

        // tekstualna, kodirana reprezentacija potpisanog Access JWT tokena. Korisnik dalje koristi vrijednost za autentifikaciju prilikom pristupa API Endpointovima na Kontrolerima.
        return new JwtSecurityTokenHandler().WriteToken(jwt);
    }

    /// <summary>
    /// Creates refresh token used for issuing new access tokens to users with active sessions.
    /// </summary>
    /// <returns>Coded representation of randomized refresh token.</returns>
    public string GenerateRefreshToken()
    {
        var bytes = RandomNumberGenerator.GetBytes(48); // Refresh token je nasumicna vjerodajnica, koristi se za obnavljanje pristupa bez ponovnog login sve dok sesija ne istekne.
        return Convert.ToBase64String(bytes);
    }
}
