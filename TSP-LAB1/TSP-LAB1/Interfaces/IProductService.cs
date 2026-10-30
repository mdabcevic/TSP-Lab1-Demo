
using TSP_LAB1.DTOs;

namespace TSP_LAB1.Interfaces;

public interface IProductService
{
    Task<List<ProductResponse>> GetProductsAsync(CancellationToken ct = default);
    Task<ProductResponse?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<ProductResponse> CreateAsync(CreateProductRequest request, CancellationToken ct = default);
}
