namespace Contracts.Events;

public record PaymentProcessed
{
    public Guid OrderId { get; init; }
    public Guid TransactionId { get; init; }
}