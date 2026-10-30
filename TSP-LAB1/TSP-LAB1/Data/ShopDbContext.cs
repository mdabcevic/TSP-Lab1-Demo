using Microsoft.EntityFrameworkCore;
using TSP_LAB1.Models;


namespace TSP_LAB1.Data;

public sealed class ShopDbContext(DbContextOptions<ShopDbContext> options) : DbContext(options)
{
    public DbSet<AppUser> Users => Set<AppUser>();

    public DbSet<Product> Products => Set<Product>();

    public DbSet<CustomerOrder> Orders => Set<CustomerOrder>();

    public DbSet<UserSession> UserSessions => Set<UserSession>();

    public DbSet<UsedRefreshToken> UsedRefreshTokens => Set<UsedRefreshToken>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        ConfigureUsers(modelBuilder);
        ConfigureProducts(modelBuilder);
        ConfigureOrders(modelBuilder);
        ConfigureSessions(modelBuilder);
        ConfigureUsedRefreshTokens(modelBuilder);
    }

    private static void ConfigureUsers(ModelBuilder mb)
    {
        mb.Entity<AppUser>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.Property(x => x.Email)
                .HasMaxLength(255)
                .IsRequired();

            entity.HasIndex(x => x.Email)
                .IsUnique();

            entity.Property(x => x.PasswordHash)
                .IsRequired();

            entity.Property(x => x.Role)
                .HasMaxLength(30)
                .IsRequired();

            entity.Property(x => x.AuthVersion)
                .HasDefaultValue(1);
        });
    }

    private static void ConfigureProducts(ModelBuilder mb)
    {
        mb.Entity<Product>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.Property(x => x.Name)
                .HasMaxLength(150)
                .IsRequired();

            entity.Property(x => x.Description)
                .HasMaxLength(1000);

            entity.Property(x => x.Price)
                .HasPrecision(12, 2);

            entity.ToTable(t =>
            {
                t.HasCheckConstraint(
                    "CK_Product_Price",
                    "\"Price\" >= 0");

                t.HasCheckConstraint(
                    "CK_Product_Stock",
                    "\"StockQuantity\" >= 0");
            });
        });
    }

    private static void ConfigureOrders(ModelBuilder mb)
    {
        mb.Entity<CustomerOrder>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.Property(x => x.Description)
                .HasMaxLength(1000)
                .IsRequired();

            entity.Property(x => x.Total)
                .HasPrecision(12, 2);

            entity.HasOne(x => x.Customer)
                .WithMany(x => x.Orders)
                .HasForeignKey(x => x.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(x => x.CustomerId);

            entity.ToTable(t =>
                t.HasCheckConstraint(
                    "CK_Order_Total",
                    "\"Total\" >= 0"));
        });
    }

    private static void ConfigureSessions(ModelBuilder mb)
    {
        mb.Entity<UserSession>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.HasOne(x => x.User)
                .WithMany(x => x.Sessions)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.Property(x => x.CurrentRefreshTokenHash)
                .HasMaxLength(64)
                .IsRequired();

            entity.HasIndex(x => x.CurrentRefreshTokenHash)
                .IsUnique();

            entity.HasIndex(x => x.UserId);

            entity.Property(x => x.RevocationReason)
                .HasMaxLength(100);
        });
    }

    private static void ConfigureUsedRefreshTokens(ModelBuilder mb)
    {
        mb.Entity<UsedRefreshToken>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.HasOne(x => x.Session)
                .WithMany(x => x.UsedRefreshTokens)
                .HasForeignKey(x => x.SessionId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.Property(x => x.TokenHash)
                .HasMaxLength(64)
                .IsRequired();

            entity.HasIndex(x => x.TokenHash)
                .IsUnique();

            entity.HasIndex(x => x.SessionId);
        });
    }
}
