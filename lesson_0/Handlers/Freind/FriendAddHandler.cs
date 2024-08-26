namespace lesson_0.Handlers.Freind
{
    using Autofac;
    using lesson_0.Models;
    using lesson_0.Models.Requests.Friend;
    using MediatR;
    using Npgsql;

    public partial class FriendAddHandler : IRequestHandler<FriendAddRequest, FriendModel>
    {
        private readonly NpgsqlDataSource _dataSource;
        public FriendAddHandler(ILifetimeScope scope)
        {
            _dataSource = scope.Resolve<WriteDataSource>().DataSource;
        }

        public async Task<FriendModel> Handle(FriendAddRequest request, CancellationToken cancellationToken)
        {
            try
            {
                await using var cmd = _dataSource.CreateCommand();

                cmd.CommandText = $"INSERT INTO public.friends (UserId, FriendId)" +
                                  $" VALUES (@UserId, @FriendId)" +
                                  $" ON conflict do nothing";
                cmd.Parameters.AddWithValue("UserId", request.UserId);
                cmd.Parameters.AddWithValue("FriendId", request.FriendId);

                await cmd.ExecuteNonQueryAsync(cancellationToken);

                return new FriendModel()
                {
                    UserId = request.UserId.ToString(),
                    FriendId = request.FriendId.ToString()
                };
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
