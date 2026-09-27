using Infra;
using MassTransit;

namespace WarehouseApi;

public sealed class OrderCreatedConsumer(AppDbContext dbContext) : IConsumer<OrderCreated>
{
    public async Task Consume(ConsumeContext<OrderCreated> context)
    {
        var order = dbContext.Orders.Single(x => x.Id == context.Message.Id);

        var shipment = dbContext.Shipments.SingleOrDefault(x => x.OrderReference == order.Reference);

        if (shipment is not null)
        {
            Console.WriteLine($"Order Consumed {context.Message.Id} - Already Processed");
            return;
        }

        var newShipment = new Shipment
        {
            OrderReference = order.Reference
        };

        dbContext.Shipments.Add(newShipment);
        await dbContext.SaveChangesAsync();
        
        Console.WriteLine($"Order Consumed {context.Message.Id} - Shipping");
    }
}