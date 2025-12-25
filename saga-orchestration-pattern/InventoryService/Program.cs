using InventoryService.Infrastructure.MessageBrokers.Consumers;
using MassTransit;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMassTransit(config =>
{
    config.AddConsumer<ReserveInventoryConsumer>();
    
    var msgBrokerConfig = builder.Configuration.GetSection("MessageBroker:RabbitMQ");

    config.UsingRabbitMq((ctx, cfg) =>
    {
        cfg.Host(msgBrokerConfig["Host"], "/", host =>
        {
            host.Username(msgBrokerConfig["Username"]!);
            host.Password(msgBrokerConfig["Password"]!);
            host.ConnectionName("platypus");
        });

        cfg.ReceiveEndpoint("inventory-reservation-queue", ep =>
        {
            ep.ConfigureConsumeTopology = false;
        
            ep.Bind("inventory-exchange", b =>
            {
                b.RoutingKey = "order.reserve";
                b.ExchangeType = "topic";
            });
        
            ep.Consumer<ReserveInventoryConsumer>(ctx);
        });
    });
});


var app = builder.Build();

app.Run();