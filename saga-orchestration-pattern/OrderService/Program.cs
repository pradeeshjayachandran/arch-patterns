using Contracts.Events;
using Marten;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using OrderService.Domain.Sagas;
using OrderService.Presentation.Models;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMarten(options =>
{
    options.Connection("host=localhost;port=5432;database=mypgdb;username=admin;password=ecomdb@2025;");
});

builder.Services.AddMassTransit(busConfigurator =>
{
    busConfigurator.AddSagaStateMachine<OrderStateMachine, OrderState>()
        .MartenRepository();
    
    var msgBrokerConfig = builder.Configuration.GetSection("MessageBroker:RabbitMQ");

    busConfigurator.UsingRabbitMq((ctx, cfg) =>
    {
        cfg.Host(msgBrokerConfig["Host"], "/", host =>
        {
            host.Username(msgBrokerConfig["Username"]!);
            host.Password(msgBrokerConfig["Password"]!);
            host.ConnectionName("platypus");
        });

        cfg.ConfigureEndpoints(ctx);
    });
});

builder.Services.AddOpenApi();

var app = builder.Build();

app.MapPost("api/order/",
    async (CreateOrderRequest request,
        [FromServices] IPublishEndpoint publishEndpoint,
        [FromServices] IBus bus,
        [FromServices] ILogger<Program> logger) =>
    {
        var orderSubmittedEvent = new OrderSubmitted
        {
            OrderId = Guid.NewGuid(),
            CustomerId =  request.CustomerId,
            OrderItems = request.Items,
            TotalAmount = request.Items.Sum(p => p.UnitPrice * p.Quantity),
            OrderDate = DateTime.UtcNow
        };
        
        await publishEndpoint.Publish(orderSubmittedEvent);
    });

app.MapOpenApi();
app.MapScalarApiReference();

app.Run();