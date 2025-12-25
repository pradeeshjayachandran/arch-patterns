using Contracts.Events;
using MassTransit;

namespace OrderService.Infrastructure.MessageBrokers.Consumers;

public class OrderSubmittedConsumer(ILogger<OrderSubmittedConsumer> logger) : IConsumer<OrderSubmitted>
{
    public Task Consume(ConsumeContext<OrderSubmitted> context)
    {
        logger.LogInformation("Order consumed: Id=> {OrderId} TotalAmount => {TotalAmount}", context.Message.OrderId,
            context.Message.TotalAmount);
        return Task.CompletedTask;
    }
}