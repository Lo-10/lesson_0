namespace lesson_0.Handlers
{
    using Autofac;
    using lesson_0.Models;
    using lesson_0.Models.Requests.Post;
    using MediatR;
    using Npgsql;

    public partial class PostGetHandler : IRequestHandler<PostGetRequest, PostGetResponse>
    {
        private readonly NpgsqlDataSource _dataSource;
        public PostGetHandler(ILifetimeScope scope)
        {
            _dataSource = scope.Resolve<ReadDataSource>().DataSource;
        }

        public async Task<PostGetResponse> Handle(PostGetRequest request, CancellationToken cancellationToken)
        {
            try
            {
                await using var cmd = _dataSource.CreateCommand();

                cmd.CommandText = $"SELECT * FROM public.posts " +
                                  $"WHERE PostId = '{request.PostId}'";

                var result = new PostGetResponse();

                await using NpgsqlDataReader reader = await cmd.ExecuteReaderAsync(cancellationToken);

                while (await reader.ReadAsync())
                {
                    result.UserId = reader["UserId"] as string;
                    result.Text = reader["Text"] as string;
                    result.PostId = request.PostId;
                }

                return result;
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
