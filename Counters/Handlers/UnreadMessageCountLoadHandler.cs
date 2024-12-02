namespace lesson_0.Handlers
{
    using Autofac;
    using Counters.Models;
    using MediatR;
    using Microsoft.Extensions.Caching.Memory;
    using Npgsql;
    using System.Collections.Generic;

    /// <summary> 
    /// Обработчик запроса на "прогрев" кэша.
    /// Данный механизм позволяет перестраивать кэш из СУБД
    /// </summary>
    public partial class UnreadMessageCountLoadHandler : INotificationHandler<UnreadMessageCountLoadNotification>
    {
        private readonly NpgsqlDataSource _dataSource;
        private readonly IMemoryCache _cache;
        public UnreadMessageCountLoadHandler(ILifetimeScope scope)
        {
            _dataSource = scope.Resolve<ReadDataSource>().DataSource;
            _cache = scope.Resolve<IMemoryCache>();
        }

        public async Task Handle(UnreadMessageCountLoadNotification notification, CancellationToken cancellationToken)
        {
            try
            {
                // Находим всех пользователей, которым необходимо определить счетчики непрочитанных сообщений
                await using var cmd = _dataSource.CreateCommand();
                // [TODO] тут можно ограничить (приоритезировать) пользователей кому следует обновить счетчик непрочитанных сообщений
                // например отсеем тех, у кого нет чатов
                cmd.CommandText = "SELECT * FROM public.users WHERE userid IN (SELECT touserid as userid FROM public.dialogs)";

                var users = new List<string>();

                await using NpgsqlDataReader reader = await cmd.ExecuteReaderAsync(cancellationToken);

                while (await reader.ReadAsync())
                {
                    users.Add(reader["UserId"] as string);
                }

                foreach (var userId in users)
                {
                    await using var cmd1 = _dataSource.CreateCommand();
                    // Для оптимизации "прогрева" кэшей возьмем только тех, у кого есть непрочитанные сообщения
                    cmd1.CommandText = $"SELECT COUNT(*) FROM public.dialogs WHERE touserid = '{userId}' AND isread = false";

                    var reader1 = await cmd1.ExecuteScalarAsync(cancellationToken);

                    int unreadedMessageCount = int.Parse(reader1.ToString());

                    _cache.Set(userId, unreadedMessageCount);
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
