
using Microsoft.EntityFrameworkCore;
using TSP_LAB1.Data;
using TSP_LAB1.Interfaces;
using TSP_LAB1.Models;


namespace TSP_LAB1.Repositories;

/// <summary>
/// Performs CRUD operations for applicattion user entries against db.
/// </summary>
/// <param name="db">ORM context for database.</param>
public sealed class UserRepository(ShopDbContext db) : IUserRepository
{
    /// <summary>
    /// Find user based on unique email.
    /// </summary>
    /// <param name="email">Email to search for.</param>
    /// <param name="ct"></param>
    /// <returns>AppUser entry from database.</returns>
    public Task<AppUser?> GetByEmailAsync(string email, CancellationToken ct = default)
    {
        return db.Users.SingleOrDefaultAsync(u => u.Email == email, ct);
    }
}
