using TSP_LAB1.Models;

namespace TSP_LAB1.Interfaces;

public interface IProductRepository
{
    Task<List<Product>> GetActiveAsync(CancellationToken ct = default);
    Task<Product?> GetByIdAsync(int id, CancellationToken ct = default);
    Task AddAsync(Product product, CancellationToken ct = default);
}
