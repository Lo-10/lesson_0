namespace lesson_0.Handlers
{
    using Autofac;
    using lesson_0.Accession;
    using lesson_0.Models.Requests;
    using MediatR;
    using Microsoft.IdentityModel.Tokens;
    using Npgsql;
    using System.IdentityModel.Tokens.Jwt;

    public partial class LoginHandler : IRequestHandler<LoginRequest, LoginResponse>
    {
        private readonly IMediator _mediator;
        public LoginHandler(ILifetimeScope scope)
        {

        }

        public async Task<LoginResponse> Handle(LoginRequest request, CancellationToken cancellationToken)
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
                cmd.CommandText = $"SELECT password FROM public.users " +
                                  $"WHERE UserName = '{request.UserName}'";

                string password = "";

                NpgsqlDataReader reader = await cmd.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    password = reader["password"] as string;
                }

                if (Encryption.VerifyHashedPassword(password, request.Password))
                {
                    var now = DateTime.UtcNow;

                    AuthOptions authOptions = new();
                    // создаем JWT-токен
                    var jwt = new JwtSecurityToken(
                            issuer: AuthOptions.ISSUER,
                            audience: password,
                            notBefore: now,
                            expires: now.Add(TimeSpan.FromMinutes(AuthOptions.LIFETIME)),
                            signingCredentials: new SigningCredentials(authOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));
                    var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);
                    return new LoginResponse() { Token = encodedJwt };
                }
                else return new LoginResponse() { Token = null };

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
