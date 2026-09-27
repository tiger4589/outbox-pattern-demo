using Infra;
using MassTransit;
using Scalar.AspNetCore;
using WarehouseApi;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.AddSqlServerDbContext<AppDbContext>(
    "database");

builder.Services.AddOpenApi();

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<OrderCreatedConsumer>();

    x.AddEntityFrameworkOutbox<AppDbContext>(o =>
    {
        o.UseSqlServer();
        o.UseBusOutbox();
    });

    x.AddConfigureEndpointsCallback((context, nameof, cfg) =>
    {
        cfg.UseEntityFrameworkOutbox<AppDbContext>(context, opts =>
        {
            opts.MessageDeliveryLimit = 100;
            opts.MessageDeliveryTimeout = TimeSpan.FromSeconds(30);
        });
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

app.Run();