namespace lesson_0.Handlers
{
    using Autofac;
    using lesson_0.Accession;
    using lesson_0.Models;
    using lesson_0.Models.Requests;
    using MediatR;
    using Microsoft.IdentityModel.Tokens;
    using Npgsql;
    using System.IdentityModel.Tokens.Jwt;
    using System.Security.Claims;

    public partial class LoginHandler : IRequestHandler<LoginRequest, LoginResponse>
    {
        private readonly NpgsqlDataSource _dataSource;
        public LoginHandler(ILifetimeScope scope)
        {
            _dataSource = scope.Resolve<ReadDataSource>().DataSource;
        }

        public async Task<LoginResponse> Handle(LoginRequest request, CancellationToken cancellationToken)
        {
            try
            {
                await using var cmd = _dataSource.CreateCommand();

                cmd.CommandText = $"SELECT password, userid FROM public.users " +
                                  $"WHERE UserName = '{request.UserName}'";

                string password = "";
                string userId = "";

                NpgsqlDataReader reader = await cmd.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    password = reader["password"] as string;
                    userId = reader["userid"] as string;
                }

                if (Encryption.VerifyHashedPassword(password, request.Password))
                {
                    var now = DateTime.UtcNow;

                    AuthOptions authOptions = new();
                    // создаем JWT-токен
                    var jwt = new JwtSecurityToken(
                            issuer: AuthOptions.ISSUER,
                            audience: password,
                            claims: [new Claim("userId", userId)],
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
