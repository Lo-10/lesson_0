namespace Counters.Handlers
{
    using Autofac;
    using MediatR;
    using Npgsql;
    using Counters.Models;
    using Microsoft.Extensions.Caching.Memory;

    public partial class IncreaseUnreadMessageCounterHandler : IRequestHandler<IncreaseUnreadMessageCounterRequest, bool>
    {
        private readonly NpgsqlDataSource _dataSource;
        private readonly ILogger<IncreaseUnreadMessageCounterHandler> _logger;
        private readonly IMemoryCache _cache;

        public IncreaseUnreadMessageCounterHandler(ILifetimeScope scope)
        {
            _dataSource = scope.Resolve<WriteDataSource>().DataSource;
            _logger = scope.Resolve<ILogger<IncreaseUnreadMessageCounterHandler>>();
            _cache = scope.Resolve<IMemoryCache>();
        }

        public async Task<bool> Handle(IncreaseUnreadMessageCounterRequest request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("{HandlerName}:{RequestId} Enter with request {@Request}", GetType().Name, request.RequestId, request);
            try
            {
                // [TODO] провешиваем блокировку на изменение счетчика, на случай конкурентного доступа на запись
                _cache.TryGetValue<int?>(request.UserId, out var unreadedMessageCount);

                // Если счетчика еще не было в кэше
                if (unreadedMessageCount == null)
                {
                    unreadedMessageCount = 0;
                }

                // Увеличиваем значение счетчика
                unreadedMessageCount += 1;
                _cache.Set(request.UserId, unreadedMessageCount);
                // [TODO] снимаем блокировку

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{HandlerName}::{RequestId} Error: {ErrorMessage}", GetType().Name, request.RequestId, ex.Message);
                return false;
            }
            finally
            {
                _logger.LogInformation("{HandlerName}:{RequestId} Exit", GetType().Name, request.RequestId);
            }
        }
    }
}
