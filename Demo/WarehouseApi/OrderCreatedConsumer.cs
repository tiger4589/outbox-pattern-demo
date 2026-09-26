using Infra;
using MassTransit;

namespace WarehouseApi;

public sealed class OrderCreatedConsumer : IConsumer<OrderCreated>
{
    public Task Consume(ConsumeContext<OrderCreated> context)
    {
        Console.WriteLine($"Order Consumed {context.Message.Id}");
        return Task.CompletedTask;
    }
}