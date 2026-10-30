namespace TSP_LAB1.Models;

public sealed class UsedRefreshToken
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid SessionId { get; set; }

    public UserSession Session { get; set; } = null!;

    public required string TokenHash { get; set; }

    public DateTimeOffset UsedAt { get; set; }
        = DateTimeOffset.UtcNow;
}