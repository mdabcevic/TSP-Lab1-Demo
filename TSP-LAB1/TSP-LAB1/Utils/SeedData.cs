
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TSP_LAB1.Data;
using TSP_LAB1.Models;


namespace TSP_LAB1.Utils;

/// <summary>
/// Helper class for seeding some initial data into the database.
/// Runs once only on startup (i.e. early return if there are already some users available.)
/// </summary>
public static class SeedData
{
    public static async Task InitializeAsync(ShopDbContext db)
    {
        if (await db.Users.AnyAsync())
            return;

        // Radi jednostavnosti testiranja login u 1. lab vježbi, lozinke su u čitkom formatu.
        //var hasher = new PasswordHasher<AppUser>();

        var alice = CreateUser(
            "10000000-0000-0000-0000-000000000001",
            "alice@example.test",
            "Customer");

        var bob = CreateUser(
            "10000000-0000-0000-0000-000000000002",
            "bob@example.test",
            "Customer");

        var admin = CreateUser(
            "10000000-0000-0000-0000-000000000003",
            "admin@example.test",
            "Admin");

        foreach (var user in new[] { alice, bob, admin })
            user.PasswordHash = "LabPass123!";

        db.Users.AddRange(alice, bob, admin);

        db.Products.AddRange(
            new Product
            {
                Id = 1,
                Name = "Mechanical Keyboard",
                Description = "USB mechanical keyboard",
                Price = 89.90m,
                StockQuantity = 20
            },
            new Product
            {
                Id = 2,
                Name = "Wireless Mouse",
                Description = "Bluetooth mouse",
                Price = 35.50m,
                StockQuantity = 40
            },
            new Product
            {
                Id = 3,
                Name = "USB-C Hub",
                Description = "Multiport USB-C adapter",
                Price = 49.00m,
                StockQuantity = 15
            });

        db.Orders.AddRange(
            new CustomerOrder
            {
                Id = 101,
                CustomerId = alice.Id,
                Description = "Mechanical Keyboard",
                Total = 89.90m
            },
            new CustomerOrder
            {
                Id = 102,
                CustomerId = bob.Id,
                Description = "Wireless Mouse",
                Total = 35.50m
            });

        await db.SaveChangesAsync();
    }

    private static AppUser CreateUser(string id, string email, string role)
    {
        return new AppUser
        {
            Id = Guid.Parse(id),
            Email = email,
            Role = role,
            PasswordHash = string.Empty,
            IsActive = true,
            AuthVersion = 1
        };
    }
}
