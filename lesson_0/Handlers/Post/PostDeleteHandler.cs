namespace lesson_0.Handlers.Freind
{
    using Autofac;
    using lesson_0.Models;
    using lesson_0.Models.Requests.Post;
    using MediatR;
    using Npgsql;

    public partial class PostDeleteHandler : IRequestHandler<PostDeleteRequest, bool?>
    {
        private readonly NpgsqlDataSource _dataSource;
        public PostDeleteHandler(ILifetimeScope scope)
        {
            _dataSource = scope.Resolve<WriteDataSource>().DataSource;
        }

        public async Task<bool?> Handle(PostDeleteRequest request, CancellationToken cancellationToken)
        {
            try
            {
                await using var cmd = _dataSource.CreateCommand();

                cmd.CommandText = $"DELETE FROM public.posts" +
                                  $" WHERE PostId = @PostId";
                cmd.Parameters.AddWithValue("PostId", request.PostId.ToString());

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
