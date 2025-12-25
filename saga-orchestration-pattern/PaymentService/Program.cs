using MassTransit;
using PaymentService.Infrastructure.MessageBrokers.Consumers;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMassTransit(config =>
{
    config.AddConsumer<ProcessPaymentConsumer>();
    
    var msgBrokerConfig = builder.Configuration.GetSection("MessageBroker:RabbitMQ");

    config.UsingRabbitMq((ctx, cfg) =>
    {
        cfg.Host(msgBrokerConfig["Host"], "/", host =>
        {
            host.Username(msgBrokerConfig["Username"]!);
            host.Password(msgBrokerConfig["Password"]!);
            host.ConnectionName("platypus");
        });

        cfg.ReceiveEndpoint("payment-process-queue", ep =>
        {
            ep.ConfigureConsumeTopology = false;
        
            ep.Bind("payment-exchange", b =>
            {
                b.RoutingKey = "order.payment";
                b.ExchangeType = "topic";
            });
        
            ep.Consumer<ProcessPaymentConsumer>(ctx);
        });
    });
});


var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.Run();