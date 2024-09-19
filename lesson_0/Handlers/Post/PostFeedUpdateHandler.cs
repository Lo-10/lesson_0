namespace lesson_0.Handlers
{
    using Autofac;
    using lesson_0.Models;
    using lesson_0.Models.Requests.Post;
    using lesson_0.Services;
    using MediatR;
    using Microsoft.Extensions.Caching.Memory;
    using Npgsql;

    public partial class PostFeedUpdateHandler : INotificationHandler<PostFeedUpdateNotification>
    {
        private readonly NpgsqlDataSource _dataSource;
        private readonly IMemoryCache _cache;
        private readonly IEventBusService _eventBus;
        public PostFeedUpdateHandler(ILifetimeScope scope)
        {
            _dataSource = scope.Resolve<ReadDataSource>().DataSource;
            _cache = scope.Resolve<IMemoryCache>();
            _eventBus = scope.Resolve<IEventBusService>();
        }

        public async Task Handle(PostFeedUpdateNotification notification, CancellationToken cancellationToken)
        {
            try
            {
                var post = notification.Post;
                // Находим всех подписчиков, которым необходимо перестроить ленты постов
                await using var cmd = _dataSource.CreateCommand();

                cmd.CommandText = $"SELECT * FROM public.friends " +
                                  $"WHERE FriendId = '{post.UserId}'";

                var friends = new List<string>();

                await using NpgsqlDataReader reader = await cmd.ExecuteReaderAsync(cancellationToken);

                while (await reader.ReadAsync())
                {
                    friends.Add(reader["UserId"] as string);
                }

                // Перестраиваем ленту подписчиков
                foreach (var userId in friends)
                {
                    _cache.TryGetValue(userId, out List<PostModel> feed);
                    feed ??= [];
                    // Если в ленте уже есть пост (требуется обновление) - заменяем
                    feed.RemoveAll(p => p.PostId == post.PostId);
                    feed.Add(post);
                    _cache.Set(userId, feed.Take(1000).OrderByDescending(p=>p.PostCreatedAt).ToList());
                    // отправляем пост подписчику через websocket
                    await _eventBus.SendMessageAsync(userId, post, cancellationToken);                    
                }
            }
            catch (Exception ex)
            {

            }
            finally
            {

            }
        }
    }
}
