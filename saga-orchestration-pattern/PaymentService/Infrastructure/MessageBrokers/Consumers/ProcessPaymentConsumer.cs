using Contracts.Commands;
using Contracts.Events;
using MassTransit;

namespace PaymentService.Infrastructure.MessageBrokers.Consumers;

public class ProcessPaymentConsumer(ILogger<ProcessPaymentConsumer> logger) : IConsumer<ProcessPayment>
{
    public async Task Consume(ConsumeContext<ProcessPayment> context)
    {
        logger.LogInformation("Payment processed successfully for order - {OrderId}", context.Message.OrderId);

        await context.Publish(new PaymentProcessed
        {
            OrderId = context.Message.OrderId,
            TransactionId = Guid.NewGuid(),
        });
    }
}