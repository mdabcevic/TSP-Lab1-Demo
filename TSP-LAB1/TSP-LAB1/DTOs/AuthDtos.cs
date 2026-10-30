
using System.ComponentModel.DataAnnotations;

namespace TSP_LAB1.DTOs;

public sealed record LoginRequest(
    [Required, EmailAddress] string Email,
    [Required] string Password);

public sealed record TokenResponse(
    string AccessToken,
    string RefreshToken,
    string TokenType,
    DateTimeOffset ExpiresAt);

public sealed record RefreshTokenRequest(
    [property: Required] string RefreshToken);

