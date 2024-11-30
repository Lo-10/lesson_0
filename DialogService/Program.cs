using Autofac;
using Autofac.Extensions.DependencyInjection;
using Dialogs;
using Dialogs.Handlers;
using Dialogs.Models;
using Dialogs.Services;
using MediatR;
using Npgsql;

var builder = WebApplication.CreateBuilder(args);

builder.Host
    .UseServiceProviderFactory(new AutofacServiceProviderFactory())
    .ConfigureContainer<ContainerBuilder>(builder =>
    {       
        builder.Register((c, p) => new SendMessageHandler(c.Resolve<ILifetimeScope>()))
           .As<IRequestHandler<Dialogs.Models.SendMessageRequest, bool?>>();
        builder.Register((c, p) => new DialogGetHandler(c.Resolve<ILifetimeScope>()))
           .As<IRequestHandler<Dialogs.Models.DialogGetRequest, DialogMessage[]>>();

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
    });

// Add services to the container.
builder.Services.AddGrpc();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapGrpcService<Dialogs.Services.DialogService> ();
app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();
