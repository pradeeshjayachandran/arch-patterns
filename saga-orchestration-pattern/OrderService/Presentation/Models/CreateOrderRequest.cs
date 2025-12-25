using Contracts.Entities;

namespace OrderService.Presentation.Models;

public record CreateOrderRequest()
{
    public required string CustomerId { get; init; }
    public List<Product> Items { get; init; } = [];
}