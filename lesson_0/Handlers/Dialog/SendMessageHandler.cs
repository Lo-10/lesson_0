namespace lesson_0.Handlers
{
    using Autofac;
    using CountersClient;
    using DialogsClient;
    using Grpc.Net.Client;
    using lesson_0.Models;
    using MediatR;
    using Microsoft.IdentityModel.Tokens;
    using Npgsql;
    using SendMessageRequest = Models.Requests.Dialog.SendMessageRequest;

    public partial class SendMessageHandler : IRequestHandler<SendMessageRequest, string?>
    {
        private readonly NpgsqlDataSource _dataSource;
        private readonly ILogger<SendMessageHandler> _logger;

        public SendMessageHandler(ILifetimeScope scope)
        {
            _dataSource = scope.Resolve<WriteDataSource>().DataSource;
            _logger = scope.Resolve<ILogger<SendMessageHandler>>();
        }

        public async Task<string?> Handle(SendMessageRequest request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("{HandlerName}:{RequestId} Enter with request {@Request}", GetType().Name, request.RequestId, request);
            try
            {
                // Сага создания сообщения
                // 1. Создаем новое сообщение в сервисе диалогов
                using var channelDialogs = GrpcChannel.ForAddress(Environment.GetEnvironmentVariables()["dialogs_grpc_url"].ToString());

                var clientDialogs = new DialogService.DialogServiceClient(channelDialogs);

                var reply = await clientDialogs.MessageSendAsync(new DialogsClient.SendMessageRequest
                {
                    FromUserId = request.FromUserId.ToString(),
                    ToUserId = request.ToUserId.ToString(),
                    Text = request.Text,
                    RequestId = request.RequestId
                });

                var messageId = reply.MessageId;

                // 2. Если сообщение успешно создано, то увеличиваем счетчик непрочитанных сообщений пользователя
                if (!messageId.IsNullOrEmpty())
                {
                    using var channelCounters = GrpcChannel.ForAddress(Environment.GetEnvironmentVariables()["counters_grpc_url"].ToString());

                    var clientCounters = new CounterService.CounterServiceClient(channelCounters);

                    var replyCounters = await clientCounters.IncreaseUnreadMessageCounterAsync(new MessageCreatedRequest
                    {
                        FromUserId = request.FromUserId.ToString(),
                        ToUserId = request.ToUserId.ToString(),
                        MessageId = messageId,
                        RequestId = request.RequestId
                    });
                }
                // 3. [TODO] если ошибка в п.2 - генерируем компенсирующее событие
                return reply.MessageId;
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
