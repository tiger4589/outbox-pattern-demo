var builder = DistributedApplication.CreateBuilder(args);

var rabbitmq = builder.AddRabbitMQ("rabbitmq")
    .WithManagementPlugin();

var sql = builder.AddSqlServer("sql");

var db = sql.AddDatabase("database");

var migrations = builder.AddProject<Projects.DbOrchestrator>("dborchestrator")
    .WithReference(db)
    .WaitFor(db);

builder.AddProject<Projects.OrderApi>("orderapi")
    .WithReference(db)
    .WaitFor(db)
    .WithReference(rabbitmq)
    .WaitFor(rabbitmq);

builder.AddProject<Projects.WarehouseApi>("warehouseapi")
    .WithReference(db)
    .WaitFor(db)
    .WithReference(rabbitmq)
    .WaitFor(rabbitmq);


builder.Build().Run();
