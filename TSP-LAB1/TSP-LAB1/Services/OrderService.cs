
using TSP_LAB1.DTOs;
using TSP_LAB1.Interfaces;
using TSP_LAB1.Models;

namespace TSP_LAB1.Services;

/// <summary>
/// Business Layer for operations on order resource.
/// </summary>
/// <param name="orders">Data Layer for CRUD against order entries in DB.</param>
public sealed class OrderService(IOrderRepository orders) : IOrderService
{
    /// <summary>
    /// Find single order based on ID.
    /// </summary>
    /// <param name="id">ID of the order entry to be checked for in DB.</param>
    /// <param name="ct"></param>
    /// <returns>Response with order info.</returns>
    public async Task<OrderResponse?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var order = await orders.GetByIdAsync(id, ct);
        return order is null
            ? null
            : ToResponse(order);
    }

    /// <summary>
    /// Find all order entries in DB.
    /// </summary>
    /// <param name="ct"></param>
    /// <returns>List of all orders.</returns>
    public async Task<List<OrderResponse>> GetAllAsync(CancellationToken ct = default)
    {
        var ordersList = await orders.GetAllAsync(ct);
        return [.. ordersList.Select(ToResponse)];
    }

    private static OrderResponse ToResponse(CustomerOrder o) => new(o.Id, o.Description, o.Total, o.CreatedAt);
}
