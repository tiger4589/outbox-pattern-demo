using Infra;
using MassTransit;

namespace WarehouseApi;

public sealed class OrderCreatedConsumer : IConsumer<OrderCreated>
{
    public Task Consume(ConsumeContext<OrderCreated> context)
    {
        // Fetch the order details you need from the orders table

        // Check if the order reference is already in the warehouse database to avoid duplicate processing and keep the service idempotent. If the order is not found, proceed to process it.

        // If it's a new message - Run the business logic to process the order and update the warehouse database accordingly. This may include reserving stock, updating inventory levels, and preparing the order for shipment.

        Console.WriteLine($"Order Consumed {context.Message.Id} - Shipping");
        return Task.CompletedTask;
    }
}