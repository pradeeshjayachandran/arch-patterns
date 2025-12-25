using Contracts.Entities;

namespace Contracts.Events;

public record OrderSubmitted
{
    public Guid OrderId { get; init; }
    public required string CustomerId { get; set; }
    public required List<Product> OrderItems { get; init; }
    public decimal TotalAmount { get; init; }
    public required DateTime OrderDate { get; init; }
}