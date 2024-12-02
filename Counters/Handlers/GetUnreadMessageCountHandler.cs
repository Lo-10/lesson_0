namespace Counters.Handlers
{
    using Autofac;
    using Counters.Models;
    using MediatR;
    using Microsoft.Extensions.Caching.Memory;
    using Npgsql;

    public partial class GetUnreadMessageCountHandler : IRequestHandler<GetUnreadMessageCountRequest, int?>
    {
        private readonly NpgsqlDataSource _dataSource;
        private readonly ILogger<GetUnreadMessageCountHandler> _logger;
        private readonly IMemoryCache _cache;
        public GetUnreadMessageCountHandler(ILifetimeScope scope)
        {
            _dataSource = scope.Resolve<ReadDataSource>().DataSource;
            _logger = scope.Resolve<ILogger<GetUnreadMessageCountHandler>>();
            _cache = scope.Resolve<IMemoryCache>();
        }

        public async Task<int?> Handle(GetUnreadMessageCountRequest request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("{HandlerName}:{RequestId} Enter with request {@Request}", GetType().Name, request.RequestId, request);
            try
            {
                _cache.TryGetValue<int?>(request.UserId.ToString(), out var unreadedMessageCount);

                if (unreadedMessageCount == null)
                {
                    return 0;
                }
                else
                {
                    return unreadedMessageCount;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{HandlerName}::{RequestId} Error: {ErrorMessage}", GetType().Name, request.RequestId, ex.Message);
                return null;
            }
            finally
            {
                _logger.LogInformation("{HandlerName}:{RequestId} Exit", GetType().Name, request.RequestId);
            }
        }
    }
}
