
using System.ComponentModel.DataAnnotations;

namespace TSP_LAB1.DTOs;

public sealed record ProductResponse(
    int Id,
    string Name,
    string? Description,
    decimal Price,
    int StockQuantity);

public sealed record CreateProductRequest(
    [Required, StringLength(150)]
    string Name,

    [StringLength(1000)]
    string? Description,

    [Range(typeof(decimal), "0", "9999999999")]
    decimal Price,

    [Range(0, int.MaxValue)]
    int StockQuantity);
