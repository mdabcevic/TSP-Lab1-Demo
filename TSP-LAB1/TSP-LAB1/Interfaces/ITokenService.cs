using TSP_LAB1.Models;

namespace TSP_LAB1.Interfaces;

public interface ITokenService
{
    // TODO: dovršiti implementaciju funkcionalnosti za stvaranje ispravnog JWT Access Tokena
    string CreateAccessToken(AppUser user, Guid sessionId);
    string GenerateRefreshToken();
    //string HashRefreshToken(string refreshToken);
}
