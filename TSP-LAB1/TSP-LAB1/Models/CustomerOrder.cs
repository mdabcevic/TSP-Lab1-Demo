namespace TSP_LAB1.Models;

public sealed class CustomerOrder
{
    public int Id { get; set; }

    public Guid CustomerId { get; set; }

    public AppUser Customer { get; set; } = null!;

    public required string Description { get; set; }

    public decimal Total { get; set; }

    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
}