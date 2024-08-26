using Autofac;
using Autofac.Extensions.DependencyInjection;
using lesson_0.Accession;
using lesson_0.Handlers;
using lesson_0.Handlers.Freind;
using lesson_0.Models;
using lesson_0.Models.Requests;
using lesson_0.Models.Requests.Friend;
using lesson_0.Models.Requests.Post;
using lesson_0.Services;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Npgsql;
using Swashbuckle.AspNetCore.Filters;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

builder.Host
    .UseServiceProviderFactory(new AutofacServiceProviderFactory())
    .ConfigureContainer<ContainerBuilder>(builder =>
    {
        builder.Register((c, p) => new UserRegisterHandler(c.Resolve<ILifetimeScope>()))
                   .As<IRequestHandler<UserRegisterRequest, UserRegisterResponse>>();
        builder.Register((c, p) => new LoginHandler(c.Resolve<ILifetimeScope>()))
           .As<IRequestHandler<LoginRequest, LoginResponse>>();
        builder.Register((c, p) => new UserGetHandler(c.Resolve<ILifetimeScope>()))
           .As<IRequestHandler<UserGetRequest, UserModel>>();
        builder.Register((c, p) => new UserSearchHandler(c.Resolve<ILifetimeScope>()))
           .As<IRequestHandler<UserSearchRequest, UserModel>>();
        builder.Register((c, p) => new FriendAddHandler(c.Resolve<ILifetimeScope>()))
           .As<IRequestHandler<FriendAddRequest, FriendModel>>();
        builder.Register((c, p) => new FriendDeleteHandler(c.Resolve<ILifetimeScope>()))
           .As<IRequestHandler<FriendDeleteRequest, bool?>>();
        builder.Register((c, p) => new PostCreateHandler(c.Resolve<ILifetimeScope>()))
           .As<IRequestHandler<PostCreateRequest, PostModel>>();
        builder.Register((c, p) => new PostUpdateHandler(c.Resolve<ILifetimeScope>()))
           .As<IRequestHandler<PostUpdateRequest, bool?>>();
        builder.Register((c, p) => new PostGetHandler(c.Resolve<ILifetimeScope>()))
           .As<IRequestHandler<PostGetRequest, PostGetResponse>>();
        builder.Register((c, p) => new PostDeleteHandler(c.Resolve<ILifetimeScope>()))
           .As<IRequestHandler<PostDeleteRequest, bool?>>();
        builder.Register((c, p) => new PostFeedGetHandler(c.Resolve<ILifetimeScope>()))
           .As<IRequestHandler<PostFeedGetRequest, PostModel[]>>();
        builder.Register((c, p) => new PostFeedUpdateHandler(c.Resolve<ILifetimeScope>()))
           .As<INotificationHandler<PostFeedUpdateNotification>>();
        builder.Register((c, p) => new PostFeedLoadHandler(c.Resolve<ILifetimeScope>()))
           .As<INotificationHandler<PostFeedLoadNotification>>();

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

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                    .AddJwtBearer(options =>
                    {
                        options.RequireHttpsMetadata = false;
                        AuthOptions authOptions = new();
                        options.TokenValidationParameters = new TokenValidationParameters
                        {
                            ValidateIssuer = true,
                            ValidIssuer = AuthOptions.ISSUER,
                            ValidateAudience = false,
                            ValidAudience = AuthOptions.AUDIENCE,
                            ValidateLifetime = true,
                            IssuerSigningKey = authOptions.GetSymmetricSecurityKey(),
                            ValidateIssuerSigningKey = true,
                        };
                    });

builder.Services.AddSwaggerGen(setup =>
{
    setup.SwaggerDoc("v1", new OpenApiInfo()
    {
        Title = "OTUS Highload Architect",
        Version = "1.2.0"
    });
    setup.OperationFilter<AddResponseHeadersFilter>();
    setup.ExampleFilters();
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    setup.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));

    // Include 'SecurityScheme' to use JWT Authentication
    var jwtSecurityScheme = new OpenApiSecurityScheme
    {
        Scheme = "bearer",
        BearerFormat = "JWT",
        Name = "JWT Authentication",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Description = "Put **_ONLY_** your JWT Bearer token on textbox below!",

        Reference = new OpenApiReference
        {
            Id = JwtBearerDefaults.AuthenticationScheme,
            Type = ReferenceType.SecurityScheme
        }
    };
    setup.AddSecurityDefinition(jwtSecurityScheme.Reference.Id, jwtSecurityScheme);
    setup.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                { jwtSecurityScheme, Array.Empty<string>() }
            });
});
builder.Services.AddSwaggerExamples();
builder.Services.AddMemoryCache();
builder.Services.AddHostedService<QueuedPostFeedUpdateService>();
builder.Services.AddHostedService<PostFeedLoadService>();
builder.Services.AddSingleton<IBackgroundTaskQueue>(_ => new PostFeedUpdateQueue(10000000));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

}

app.UseAuthorization();

app.MapControllers();

app.Run();
