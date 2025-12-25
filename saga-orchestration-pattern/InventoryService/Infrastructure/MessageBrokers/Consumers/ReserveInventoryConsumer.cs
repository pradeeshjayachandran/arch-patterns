using Contracts.Commands;
using Contracts.Events;
using MassTransit;

namespace InventoryService.Infrastructure.MessageBrokers.Consumers;

public class ReserveInventoryConsumer(ILogger<ReserveInventoryConsumer> logger) : IConsumer<ReserveInventory>
{
    public async Task Consume(ConsumeContext<ReserveInventory> context)
    {
        logger.LogInformation("Reserved Inventory for order {OrderId}", context.Message.OrderId);
        
        await context.Publish(new InventoryReserved
        {
            OrderId = context.Message.OrderId,
            ReservationId = Guid.NewGuid()
        });
    }
}