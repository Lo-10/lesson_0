namespace lesson_0.Handlers
{
    using Autofac;
    using lesson_0.Accession;
    using lesson_0.Models;
    using lesson_0.Models.Requests;
    using MediatR;
    using Npgsql;

    public partial class UserRegisterHandler : IRequestHandler<UserRegisterRequest, UserRegisterResponse>
    {
        private readonly NpgsqlDataSource _dataSource;
        public UserRegisterHandler(ILifetimeScope scope)
        {
            _dataSource = scope.Resolve<WriteDataSource>().DataSource;
        }

        public async Task<UserRegisterResponse> Handle(UserRegisterRequest request, CancellationToken cancellationToken)
        {
            try
            {
                await using var cmd = _dataSource.CreateCommand();

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
