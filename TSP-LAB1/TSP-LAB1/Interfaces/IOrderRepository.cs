using TSP_LAB1.Models;

namespace TSP_LAB1.Interfaces;

public interface IOrderRepository
{
    Task<CustomerOrder?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<List<CustomerOrder>> GetAllAsync(CancellationToken ct = default);
}