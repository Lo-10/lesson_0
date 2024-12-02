using Autofac;
using Autofac.Extensions.DependencyInjection;
using Counters.Handlers;
using Counters.Models;
using Counters.Services;
using lesson_0.Handlers;
using MediatR;
using Microsoft.Extensions.Caching.Memory;
using Npgsql;

var builder = WebApplication.CreateBuilder(args);

builder.Host
    .UseServiceProviderFactory(new AutofacServiceProviderFactory())
    .ConfigureContainer<ContainerBuilder>(builder =>
    {
        builder.Register((c, p) => new GetUnreadMessageCountHandler(c.Resolve<ILifetimeScope>()))
           .As<IRequestHandler<GetUnreadMessageCountRequest, int?>>();
        builder.Register((c, p) => new IncreaseUnreadMessageCounterHandler(c.Resolve<ILifetimeScope>()))
           .As<IRequestHandler<IncreaseUnreadMessageCounterRequest, bool>>();
        builder.Register((c, p) => new UnreadMessageCountLoadHandler(c.Resolve<ILifetimeScope>()))
           .As<INotificationHandler<UnreadMessageCountLoadNotification>>();


        builder.Register((c, p) =>
        {
            var pgServer = Environment.GetEnvironmentVariables()["pgsql_server_master"];
            var pgPort = Environment.GetEnvironmentVariables()["pgsql_port_master"];
            var pgDb = Environment.GetEnvironmentVariables()["pgsql_db"];
            var pgUser = Environment.GetEnvironmentVariables()["pgsql_user"];
            var pgPassord = Environment.GetEnvironmentVariables()["pgsql_password"];
            var connectionString = $"Server={pgServer};Port={pgPort};Username={pgUser};Password={pgPassord};Database={pgDb}";

            var dataSourceBuilder = new NpgsqlDataSourceBuilder(connectionString);

            var dataSource = dataSourceBuilder.Build();
            return new WriteDataSource(dataSource);
        }).As(typeof(WriteDataSource)).SingleInstance();

        builder.Register((c, p) =>
        {
            var pgServer = Environment.GetEnvironmentVariables()["pgsql_server_slave"];
            var pgPort = Environment.GetEnvironmentVariables()["pgsql_port_slave"];
            var pgDb = Environment.GetEnvironmentVariables()["pgsql_db"];
            var pgUser = Environment.GetEnvironmentVariables()["pgsql_user"];
            var pgPassord = Environment.GetEnvironmentVariables()["pgsql_password"];
            var connectionString = $"Server={pgServer};Port={pgPort};Username={pgUser};Password={pgPassord};Database={pgDb}";

            var dataSourceBuilder = new NpgsqlDataSourceBuilder(connectionString);

            var dataSource = dataSourceBuilder.Build();
            return new ReadDataSource(dataSource);
        }).As(typeof(ReadDataSource)).SingleInstance();

        builder.RegisterType<Mediator>()
               .As<IMediator>()
               .SingleInstance();

        builder.RegisterType<MemoryCache>()
               .As<IMemoryCache>()
               .SingleInstance();
    });

// Add services to the container.
builder.Services.AddGrpc();
builder.Services.AddHostedService<UnreadMessageCountLoadService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapGrpcService<CounterService>();
app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();
