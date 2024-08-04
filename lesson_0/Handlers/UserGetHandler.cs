namespace lesson_0.Handlers
{
    using Autofac;
    using lesson_0.Models;
    using MediatR;
    using Npgsql;

    public partial class UserGetHandler : IRequestHandler<UserGetRequest, UserModel>
    {
        private readonly NpgsqlDataSource _dataSource;
        public UserGetHandler(ILifetimeScope scope)
        {
            _dataSource = scope.Resolve<ReadDataSource>().DataSource;
        }

        public async Task<UserModel> Handle(UserGetRequest request, CancellationToken cancellationToken)
        {
            try
            {
                await using var cmd = _dataSource.CreateCommand();

                cmd.CommandText = $"SELECT * FROM public.users " +
                                  $"WHERE UserId = '{request.UserId}'";

                var user = new UserModel();

                await using NpgsqlDataReader reader = await cmd.ExecuteReaderAsync();

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
