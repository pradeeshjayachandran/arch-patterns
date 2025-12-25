namespace Contracts.Events;

public record InventoryReserved
{
    public Guid OrderId { get; init; }
    public Guid ReservationId { get; init; }
}