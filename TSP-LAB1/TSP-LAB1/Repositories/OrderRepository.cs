
using Microsoft.EntityFrameworkCore;
using TSP_LAB1.Data;
using TSP_LAB1.Interfaces;
using TSP_LAB1.Models;

namespace TSP_LAB1.Repositories;

/// <summary>
/// Performs CRUD operations for Order entries.
/// </summary>
/// <param name="db">ORM Database context.</param>
public sealed class OrderRepository(ShopDbContext db) : IOrderRepository
{
    /// <summary>
    /// Find single order entry.
    /// </summary>
    /// <param name="id">ID of the order to be fetched.</param>
    /// <param name="ct"></param>
    /// <returns>Order entry from database.</returns>
    public Task<CustomerOrder?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        return db.Orders
            .AsNoTracking()
            .SingleOrDefaultAsync(o => o.Id == id, ct);
    }

    /// <summary>
    /// Find all orders.
    /// </summary>
    /// <param name="ct"></param>
    /// <returns>List of all found orders.</returns>
    public Task<List<CustomerOrder>> GetAllAsync(CancellationToken ct = default)
    {
        return db.Orders
            .AsNoTracking()
            .OrderBy(o => o.Id)
            .ToListAsync(ct);
    }
}
