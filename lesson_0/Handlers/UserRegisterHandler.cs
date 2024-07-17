namespace lesson_0.Handlers
{
    using Autofac;
    using lesson_0.Accession;
    using lesson_0.Models.Requests;
    using MediatR;
    using Npgsql;
    using System.Net.Security;
    using System.Security.Cryptography;

    public partial class UserRegisterHandler : IRequestHandler<UserRegisterRequest, UserRegisterResponse>
    {
        private readonly IMediator _mediator;

        public UserRegisterHandler(ILifetimeScope scope)
        {

        }

        public async Task<UserRegisterResponse> Handle(UserRegisterRequest request, CancellationToken cancellationToken)
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
                cmd.CommandText = $"INSERT INTO public.users (UserId, UserName, FirstName, SecondName, BirthDate, Biography, City, Password)" +
                                  $" VALUES (@UserId, @UserName, @FirstName, @SecondName,@BirthDate, @Biography, @City, @Password)";
                cmd.Parameters.AddWithValue("UserId", userId);
                cmd.Parameters.AddWithValue("UserName", request.UserName);
                cmd.Parameters.AddWithValue("FirstName", request.FirstName);
                cmd.Parameters.AddWithValue("SecondName", request.SecondName);
                cmd.Parameters.AddWithValue("BirthDate", request.BirthDate);
                cmd.Parameters.AddWithValue("Biography", request.Biography);
                cmd.Parameters.AddWithValue("City", request.City);
                cmd.Parameters.AddWithValue("Password", Encryption.HashPassword(request.Password));

                await cmd.ExecuteNonQueryAsync(cancellationToken);

                return new UserRegisterResponse() { UserId = userId.ToString() };
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("duplicate key value violates unique constraint"))
                {
                    return new UserRegisterResponse() { UserId = null };
                }
                else
                {
                    return null;
                }
            }
            finally
            {

            }
        }
    }
}
