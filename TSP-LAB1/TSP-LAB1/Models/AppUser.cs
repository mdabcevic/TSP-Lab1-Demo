namespace TSP_LAB1.Models;

public sealed class AppUser
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public required string Email { get; set; }

    public required string PasswordHash { get; set; }

    public required string Role { get; set; }

    public bool IsActive { get; set; } = true;

    public int AuthVersion { get; set; } = 1;

    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    public List<CustomerOrder> Orders { get; set; } = [];

    public List<UserSession> Sessions { get; set; } = [];
}
