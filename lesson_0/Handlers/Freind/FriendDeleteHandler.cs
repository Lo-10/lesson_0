namespace lesson_0.Handlers.Freind
{
    using Autofac;
    using lesson_0.Models;
    using lesson_0.Models.Requests.Friend;
    using MediatR;
    using Npgsql;

    public partial class FriendDeleteHandler : IRequestHandler<FriendDeleteRequest, bool?>
    {
        private readonly NpgsqlDataSource _dataSource;
        public FriendDeleteHandler(ILifetimeScope scope)
        {
            _dataSource = scope.Resolve<WriteDataSource>().DataSource;
        }

        public async Task<bool?> Handle(FriendDeleteRequest request, CancellationToken cancellationToken)
        {
            try
            {
                await using var cmd = _dataSource.CreateCommand();

                cmd.CommandText = $"DELETE FROM public.friends (UserId, FriendId)" +
                                  $" WHERE UserId = @UserId and FriendId = @FriendId";
                cmd.Parameters.AddWithValue("UserId", request.UserId);
                cmd.Parameters.AddWithValue("FriendId", request.FriendId);

                await cmd.ExecuteNonQueryAsync(cancellationToken);

                return true;
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
