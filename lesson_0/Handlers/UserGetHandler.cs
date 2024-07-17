namespace lesson_0.Handlers
{
    using Autofac;
    using lesson_0.Models;
    using MediatR;
    using Npgsql;

    public partial class UserGetHandler : IRequestHandler<UserGetRequest, UserModel>
    {
        private readonly IMediator _mediator;
        public UserGetHandler(ILifetimeScope scope)
        {

        }

        public async Task<UserModel> Handle(UserGetRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var pgServer = Environment.GetEnvironmentVariables()["pgsql_server"];
                var pgPort = Environment.GetEnvironmentVariables()["pgsql_port"];
                var pgDb = Environment.GetEnvironmentVariables()["pgsql_db"];
                var pgUser = Environment.GetEnvironmentVariables()["pgsql_user"];
                var pgPassord = Environment.GetEnvironmentVariables()["pgsql_password"];
                var connectionString = $"Server={pgServer};Port={pgPort};Username={pgUser};Password={pgPassord};Database={pgDb}";

                await using var dataSource = NpgsqlDataSource.Create(connectionString);

                await using var cmd = dataSource.CreateCommand("SELECT * FROM users");

                var userId = Guid.NewGuid();
                cmd.CommandText = $"SELECT * FROM public.users " +
                                  $"WHERE UserId = '{request.UserId}'";

                var user = new UserModel();

                NpgsqlDataReader reader = await cmd.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    user.UserId = reader["UserId"] as string;
                    user.FirstName = reader["FirstName"] as string;
                    user.SecondName = reader["SecondName"] as string;
                    var bd = reader["BirthDate"] as string;
                    user.BirthDate = DateOnly.Parse(bd);
                    user.Biography = reader["Biography"] as string;
                    user.City = reader["City"] as string;
                }

                return user;
            }
            catch (Exception ex)
            {
                    return null;
            }
            finally
            {

            }
        }
    }
}
