
using TSP_LAB1.DTOs;

namespace TSP_LAB1.Interfaces;

public interface IOrderService
{
    Task<OrderResponse?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<List<OrderResponse>> GetAllAsync(CancellationToken ct = default);
}
