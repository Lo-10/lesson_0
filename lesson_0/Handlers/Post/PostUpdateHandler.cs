namespace lesson_0.Handlers
{
    using Autofac;
    using lesson_0.Accession;
    using lesson_0.Models;
    using lesson_0.Models.Requests.Post;
    using MediatR;
    using Npgsql;

    public partial class PostUpdateHandler : IRequestHandler<PostUpdateRequest, bool?>
    {
        private readonly NpgsqlDataSource _dataSource;
        private readonly IBackgroundTaskQueue _taskQueue;
        private readonly IMediator _mediator;
        public PostUpdateHandler(ILifetimeScope scope)
        {
            _dataSource = scope.Resolve<WriteDataSource>().DataSource;
            _taskQueue = scope.Resolve<IBackgroundTaskQueue>();
            _mediator = scope.Resolve<IMediator>();
        }

        public async Task<bool?> Handle(PostUpdateRequest request, CancellationToken cancellationToken)
        {
            try
            {
                await using var cmd = _dataSource.CreateCommand();
                var updatedAt = DateTime.UtcNow;

                cmd.CommandText = $"UPDATE public.posts SET Text = @Text, PostUpdatedAt = @PostUpdatedAt" +
                                  $" WHERE PostId = @PostId";
                cmd.Parameters.AddWithValue("Text", request.Text);
                cmd.Parameters.AddWithValue("PostUpdatedAt", updatedAt);
                cmd.Parameters.AddWithValue("PostId", request.PostId.ToString());

                await cmd.ExecuteNonQueryAsync(cancellationToken);

                var post = new PostModel()
                {
                    UserId = request.UserId,
                    Text = request.Text,
                    PostId = request.PostId,
                    PostCreatedAt = updatedAt
                };

                // Создаем задачу на обновление лент постов подписчиков
                Task task(CancellationToken cancellationToken) => _mediator.Publish(new PostFeedUpdateNotification
                {
                    Post = post
                }, cancellationToken);
                // Отправляем задачу в очередь на выполнение
                await _taskQueue.QueuePostFeedUpdateTaskAsync(task);

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
