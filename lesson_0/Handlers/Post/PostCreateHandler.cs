namespace lesson_0.Handlers
{
    using Autofac;
    using lesson_0.Accession;
    using lesson_0.Models;
    using lesson_0.Models.Requests.Post;
    using MediatR;
    using Npgsql;

    public partial class PostCreateHandler : IRequestHandler<PostCreateRequest, PostModel>
    {
        private readonly NpgsqlDataSource _dataSource;
        private readonly IBackgroundTaskQueue _taskQueue;
        private readonly IMediator _mediator;
        public PostCreateHandler(ILifetimeScope scope)
        {
            _dataSource = scope.Resolve<WriteDataSource>().DataSource;
            _taskQueue = scope.Resolve<IBackgroundTaskQueue>();
            _mediator = scope.Resolve<IMediator>();
        }

        public async Task<PostModel> Handle(PostCreateRequest request, CancellationToken cancellationToken)
        {
            try
            {
                await using var cmd = _dataSource.CreateCommand();

                var postId = Guid.NewGuid();
                var PostCreatedAt = DateTime.UtcNow;
                var PostUpdatedAt = PostCreatedAt;
                cmd.CommandText = $"INSERT INTO public.posts (UserId, Text, PostId, PostCreatedAt, PostUpdatedAt)" +
                                  $" VALUES (@UserId, @Text, @PostId, @PostCreatedAt, @PostUpdatedAt)";
                cmd.Parameters.AddWithValue("UserId", request.UserId);
                cmd.Parameters.AddWithValue("Text", request.Text);
                cmd.Parameters.AddWithValue("PostId", postId);
                cmd.Parameters.AddWithValue("PostCreatedAt", PostCreatedAt);
                cmd.Parameters.AddWithValue("PostUpdatedAt", PostUpdatedAt);

                await cmd.ExecuteNonQueryAsync(cancellationToken);

                var post = new PostModel()
                {
                    UserId = request.UserId,
                    Text = request.Text,
                    PostId = postId,
                    PostCreatedAt = PostCreatedAt,
                    PostUpdatedAt = PostUpdatedAt
                };

                // Создаем задачу на обновление лент постов подписчиков
                Task task(CancellationToken cancellationToken) => _mediator.Publish(new PostFeedUpdateNotification
                {
                    Post = post
                }, cancellationToken);
                // Отправляем задачу в очередь на выполнение
                await _taskQueue.QueuePostFeedUpdateTaskAsync(task);

                return post;
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
