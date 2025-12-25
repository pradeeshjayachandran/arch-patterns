namespace Contracts.Commands;

public record ProcessPayment
{
    public Guid OrderId { get; init; }
    public required string CustomerId { get; init; }
    public required decimal Amount { get; init; }
}