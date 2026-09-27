using Infra;
using MassTransit;
using OrderApi;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.AddSqlServerDbContext<AppDbContext>(
    "database");

builder.Services.AddOpenApi();

builder.Services.AddMassTransit(x =>
{
    x.AddEntityFrameworkOutbox<AppDbContext>(o =>
    {
        o.UseSqlServer();
        o.UseBusOutbox();
    });

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(builder.Configuration.GetConnectionString("rabbitmq"));
        cfg.ConfigureEndpoints(context);
    });
});

var app = builder.Build();

app.MapDefaultEndpoints();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

app.MapPost("/order", async (OrderContract order, IPublishEndpoint publishEndpoint, AppDbContext dbContext) =>
{
    if (string.IsNullOrWhiteSpace(order.Reference))
    {
        return Results.BadRequest("Reference is required.");
    }

    var orderEntity = new Order
    {
        Id = Guid.CreateVersion7(),
        Reference = order.Reference,
    };

    dbContext.Orders.Add(orderEntity);

    await publishEndpoint.Publish(new OrderCreated(orderEntity.Id));

    await dbContext.SaveChangesAsync();

    return Results.Created($"/order/{orderEntity.Id}", orderEntity);
})
.WithName("CreateOrder");


app.Run();
