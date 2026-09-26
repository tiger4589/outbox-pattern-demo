using Infra;
using MassTransit;
using OrderApi;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.AddSqlServerDbContext<AppDbContext>(
    "database");

builder.Services.AddOpenApi();

builder.AddMassTransitRabbitMq(
    "rabbitmq",
    massTransitConfiguration: registration =>
    {
        registration.AddEntityFrameworkOutbox<AppDbContext>(o =>
        {
            o.UseSqlServer();
            o.UseBusOutbox();
        });
    });
builder.Services.AddSingleton<IBusControl>(sp => (IBusControl)sp.GetRequiredService<IBus>());

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
    await publishEndpoint.Publish<OrderCreated>(new OrderCreated(150));
    await dbContext.SaveChangesAsync();
})
.WithName("CreateOrder");


app.Run();
