using Infra;
using MassTransit;
using Scalar.AspNetCore;
using WarehouseApi;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.AddSqlServerDbContext<AppDbContext>(
    "database");

builder.Services.AddOpenApi();

builder.AddMassTransitRabbitMq(
    "rabbitmq",
    massTransitConfiguration: registration =>
    {
        registration.AddConsumer<OrderCreatedConsumer>();

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

app.Run();