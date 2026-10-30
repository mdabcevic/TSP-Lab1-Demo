
using Microsoft.EntityFrameworkCore;
using TSP_LAB1.Data;
using TSP_LAB1.Interfaces;
using TSP_LAB1.Models;

namespace TSP_LAB1.Repositories;

/// <summary>
/// Performs CRUD operations for Product entries against db.
/// </summary>
/// <param name="db">ORM Database context.</param>
public sealed class ProductRepository(ShopDbContext db) : IProductRepository
{
    /// <summary>
    /// Find all active product entries.
    /// </summary>
    /// <param name="ct"></param>
    /// <returns>List of all active product entries in db.</returns>
    public Task<List<Product>> GetActiveAsync(CancellationToken ct = default)
    {
        return db.Products
            .AsNoTracking()
            .Where(p => p.IsActive)
            .OrderBy(p => p.Name)
            .ToListAsync(ct);
    }
    /// <summary>
    /// Find single product entry.
    /// </summary>
    /// <param name="id">ID of the entry to be fetched.</param>
    /// <param name="ct"></param>
    /// <returns>Single product entry if found in DB.</returns>
    public Task<Product?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        return db.Products
            .AsNoTracking()
            .SingleOrDefaultAsync(p => p.Id == id && p.IsActive, ct);
    }

    /// <summary>
    /// Add new product entry to the db.
    /// </summary>
    /// <param name="product">Product info for new entry.</param>
    /// <param name="ct"></param>
    /// <returns></returns>
    public async Task AddAsync(Product product, CancellationToken ct = default)
    {
        db.Products.Add(product);
        await db.SaveChangesAsync(ct);
    }
}
