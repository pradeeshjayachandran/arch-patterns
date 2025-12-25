using Contracts.Entities;

namespace Contracts.Commands;

public record ReserveInventory
{
    public Guid OrderId { get; init; }
    public required List<Product> Items { get; init; }
}