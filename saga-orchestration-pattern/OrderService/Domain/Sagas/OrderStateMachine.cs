using Contracts.Commands;
using Contracts.Entities;
using Contracts.Events;
using MassTransit;

namespace OrderService.Domain.Sagas;

public class OrderState : SagaStateMachineInstance
{
    public Guid CorrelationId { get; set; }
    public string CurrentState { get; set; } = null!;
    public Guid OrderId { get; set; }
    
    public Guid ReservationId { get; set; }
    public string CustomerId { get; set; }
    public Guid PaymentTransactionId { get; set; }
    public List<Product> OrderItems { get; set; }
    public decimal TotalPrice { get; set; }
    public DateTime OrderDate { get; set; }
}

public class OrderStateMachine : MassTransitStateMachine<OrderState>
{
    public OrderStateMachine()
    {
        InstanceState(x => x.CurrentState);

        Event(() => OrderAccepted, x => x.CorrelateById(context => context.Message.OrderId));
        Event(() => OrderItemReserved, x => x.CorrelateById(context => context.Message.OrderId));
        Event(() => PaymentCompleted, x => x.CorrelateById(context => context.Message.OrderId));

        Initially(
            When(OrderAccepted)
                .Then(context =>
                {
                    context.Saga.OrderId = context.Message.OrderId;
                    context.Saga.CustomerId = context.Message.CustomerId;
                    context.Saga.OrderItems = context.Message.OrderItems;
                    context.Saga.OrderDate = context.Message.OrderDate;
                    context.Saga.TotalPrice = context.Message.TotalAmount;
                })
                .Send(_ => new Uri("queue:inventory-reservation-queue"), context => new ReserveInventory
                {
                    OrderId = context.Saga.CorrelationId,
                    Items = context.Saga.OrderItems
                })
                .TransitionTo(AwaitingInventoryReservation)
        );

        During(AwaitingInventoryReservation,
            When(OrderItemReserved)
                .Then(context =>
                {
                    context.Saga.ReservationId = context.Message.ReservationId;
                })
                .Send(new Uri("queue:payment-process-queue"), context => new ProcessPayment
                {
                    OrderId = context.Saga.CorrelationId,
                    Amount =  context.Saga.TotalPrice,
                    CustomerId = context.Saga.CustomerId
                })
                .TransitionTo(AwaitingPayment));
        
        During(AwaitingPayment,
            When(PaymentCompleted)
                .Then(context =>
                { 
                    context.Saga.PaymentTransactionId = context.Message.TransactionId;
                })
                .TransitionTo(Processed));
    }

    #region Saga events
    public Event<OrderSubmitted> OrderAccepted { get; set; }
    public Event<InventoryReserved> OrderItemReserved { get; set; }
    public Event<PaymentProcessed> PaymentCompleted { get; set; }
    #endregion

    #region Saga states
    public State AwaitingInventoryReservation { get; set; }
    public State AwaitingPayment { get; set; }
    public State Processed { get; set; }
    #endregion
}

