namespace lesson_0.Handlers
{
    using Autofac;
    using lesson_0.Models;
    using MediatR;
    using Npgsql;

    public partial class UserSearchHandler : IRequestHandler<UserSearchRequest, UserModel>
    {
        private readonly NpgsqlDataSource _dataSource;
        public UserSearchHandler(ILifetimeScope scope)
        {
            _dataSource = scope.Resolve<NpgsqlDataSource>();
        }

        public async Task<UserModel> Handle(UserSearchRequest request, CancellationToken cancellationToken)
        {
            try
            {
                await using var cmd = _dataSource.CreateCommand("SELECT * FROM users");

                cmd.CommandText = $"SELECT * FROM public.users " +
                                  $"WHERE firstName ILIKE '%{request.FirstName}%' and secondName ILIKE '%{request.LastName}%'" +
                                  $"ORDER BY UserId";

                var user = new UserModel();

                await using NpgsqlDataReader reader = await cmd.ExecuteReaderAsync(cancellationToken);

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
