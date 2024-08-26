namespace lesson_0.Handlers
{
    using Autofac;
    using lesson_0.Models;
    using lesson_0.Models.Requests.Post;
    using MediatR;
    using Microsoft.Extensions.Caching.Memory;
    using Npgsql;

    public partial class PostFeedUpdateHandler : INotificationHandler<PostFeedUpdateNotification>
    {
        private readonly NpgsqlDataSource _dataSource;
        private readonly IMemoryCache _cache;
        public PostFeedUpdateHandler(ILifetimeScope scope)
        {
            _dataSource = scope.Resolve<ReadDataSource>().DataSource;
            _cache = scope.Resolve<IMemoryCache>();
        }

        public async Task Handle(PostFeedUpdateNotification notification, CancellationToken cancellationToken)
        {
            try
            {
                var post = notification.Post;
                // Находим всех друзей, которым необходимо перестроить ленты постов
                await using var cmd = _dataSource.CreateCommand();

                cmd.CommandText = $"SELECT * FROM public.friends " +
                                  $"WHERE FriendId = '{post.UserId}'";

                var friends = new List<string>();

                await using NpgsqlDataReader reader = await cmd.ExecuteReaderAsync(cancellationToken);

                while (await reader.ReadAsync())
                {
                    friends.Add(reader["UserId"] as string);
                }

                foreach (var userId in friends)
                {
                    _cache.TryGetValue(userId, out List<PostModel> feed);
                    feed ??= [];
                    // Если в ленте уже есть пост (требуется обновление) - заменяем
                    feed.RemoveAll(p => p.PostId == post.PostId);
                    feed.Add(post);
                    _cache.Set(userId, feed.Take(1000).OrderByDescending(p=>p.PostCreatedAt).ToList());
                    
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
