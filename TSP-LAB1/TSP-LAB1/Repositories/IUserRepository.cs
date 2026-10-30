using TSP_LAB1.Models;

namespace TSP_LAB1.Repositories;

public interface IUserRepository
{
    Task<AppUser?> GetByEmailAsync(string email, CancellationToken ct = default);
}