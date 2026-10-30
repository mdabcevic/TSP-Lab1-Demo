
using TSP_LAB1.DTOs;
using TSP_LAB1.Interfaces;
using TSP_LAB1.Models;

namespace TSP_LAB1.Services;

/// <summary>
/// Business Layer for operations on produict resource.
/// </summary>
/// <param name="products">Data Layer for CRUD against product entries in DB.</param>
public sealed class ProductService(IProductRepository products) : IProductService
{
    /// <summary>
    /// Fetch a list of all products from DB.
    /// </summary>
    /// <param name="ct"></param>
    /// <returns> list of all product entries.</returns>
    public async Task<List<ProductResponse>> GetProductsAsync(CancellationToken ct = default)
    {
        var items = await products.GetActiveAsync(ct);
        return [.. items.Select(ToResponse)];
    }

    /// <summary>
    /// Find specific product based on ID of the entry.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="ct"></param>
    /// <returns>Single product entry from DB.</returns>
    public async Task<ProductResponse?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var product = await products.GetByIdAsync(id, ct);
        return product is null
            ? null
            : ToResponse(product);
    }

    /// <summary>
    /// Create new product.
    /// </summary>
    /// <param name="request">Product information for the entry.</param>
    /// <param name="ct"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public async Task<ProductResponse> CreateAsync(CreateProductRequest request, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
            throw new ArgumentException("Product name is required.");

        if (request.Price < 0 || request.StockQuantity < 0)
            throw new ArgumentException("Invalid price or stock quantity.");

        var product = new Product
        {
            Name = request.Name.Trim(),
            Description = request.Description,
            Price = request.Price,
            StockQuantity = request.StockQuantity,
            IsActive = true
        };

        await products.AddAsync(product, ct);
        return ToResponse(product);
    }

    /// <summary>
    /// DTO Mapping from ORM object representation.
    /// </summary>
    /// <param name="p"></param>
    /// <returns></returns>
    private static ProductResponse ToResponse(Product p) => new(p.Id, p.Name, p.Description, p.Price, p.StockQuantity);
}
