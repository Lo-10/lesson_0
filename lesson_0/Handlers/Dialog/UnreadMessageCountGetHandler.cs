namespace lesson_0.Handlers
{
    using Autofac;
    using CountersClient;
    using Grpc.Net.Client;
    using lesson_0.Models;
    using lesson_0.Models.Requests.Dialog;
    using MediatR;
    using Npgsql;

    public partial class UnreadMessageCountGetHandler : IRequestHandler<UnreadMessageCountGetRequest, int?>
    {
        private readonly NpgsqlDataSource _dataSource;
        private readonly ILogger<UnreadMessageCountGetHandler> _logger;
        public UnreadMessageCountGetHandler(ILifetimeScope scope)
        {
            _dataSource = scope.Resolve<ReadDataSource>().DataSource;
            _logger = scope.Resolve<ILogger<UnreadMessageCountGetHandler>>();
        }

        public async Task<int?> Handle(UnreadMessageCountGetRequest request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("{HandlerName}:{RequestId} Enter with request {@Request}", GetType().Name, request.RequestId, request);
            try
            {
                using var channel = GrpcChannel.ForAddress(Environment.GetEnvironmentVariables()["counters_grpc_url"].ToString());

                var client = new CounterService.CounterServiceClient(channel);

                var reply = await client.GetUnreadMessageCountAsync(new GetUnreadMessageCountRequest
                {
                    UserId = request.UserId.ToString(),
                    RequestId = request.RequestId
                });

                return reply.Result;
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
