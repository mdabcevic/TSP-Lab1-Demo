namespace TSP_LAB1.DTOs;

public sealed record OrderResponse(
    int Id,
    string Description,
    decimal Total,
    DateTimeOffset CreatedAt);
