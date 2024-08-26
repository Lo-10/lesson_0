namespace lesson_0.Handlers
{
    using Autofac;
    using lesson_0.Models;
    using lesson_0.Models.Requests.Post;
    using MediatR;
    using Microsoft.Extensions.Caching.Memory;
    using Npgsql;
    using System.Collections.Generic;

    /// <summary> 
    /// Обработчик запроса на "прогрев" кэша.
    /// Данный механизм позволяет перестраивать кэш из СУБД
    /// </summary>
    public partial class PostFeedLoadHandler : INotificationHandler<PostFeedLoadNotification>
    {
        private readonly NpgsqlDataSource _dataSource;
        private readonly IMemoryCache _cache;
        public PostFeedLoadHandler(ILifetimeScope scope)
        {
            _dataSource = scope.Resolve<ReadDataSource>().DataSource;
            _cache = scope.Resolve<IMemoryCache>();
        }

        public async Task Handle(PostFeedLoadNotification notification, CancellationToken cancellationToken)
        {
            try
            {
                // Находим всех пользователей, которым необходимо перестроить ленты постов
                await using var cmd = _dataSource.CreateCommand();
                // Для оптимизации "прогрева" кэшей возьмем только тех, например, у кого есть друзья
                cmd.CommandText = "SELECT * FROM public.users WHERE userid IN (SELECT userid FROM public.friends)";

                var users = new List<string>();

                await using NpgsqlDataReader reader = await cmd.ExecuteReaderAsync(cancellationToken);

                while (await reader.ReadAsync())
                {
                    users.Add(reader["UserId"] as string);
                }

                var postFeedUpdateTasks = new List<Task>();

                foreach (var userId in users)
                {
                    await using var cmd1 = _dataSource.CreateCommand();
                    // Для оптимизации "прогрева" кэшей возьмем только тех, например, у кого есть друзья
                    cmd1.CommandText = $"SELECT * FROM public.posts WHERE userid = '{userId}'";

                    var feed = new List<PostModel>();

                    await using NpgsqlDataReader reader1 = await cmd1.ExecuteReaderAsync(cancellationToken);

                    while (await reader1.ReadAsync())
                    {
                        feed.Add(new PostModel()
                        {
                            PostId = Guid.Parse(reader1["PostId"] as string),
                            Text = reader1["Text"] as string,
                            UserId = Guid.Parse(reader1["UserId"] as string),
                            PostCreatedAt = (DateTime)reader1["PostCreatedAt"],
                            PostUpdatedAt = (DateTime)reader1["PostUpdatedAt"]
                        });
                    }

                    _cache.Set(userId, feed.Take(1000).OrderByDescending(p => p.PostCreatedAt).ToList());
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
