namespace Dialogs.Handlers
{
    using Autofac;
    using MediatR;
    using Npgsql;
    using Dialogs.Models;

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
                await using var cmd = _dataSource.CreateCommand();

                var messageCreatedAt = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();
                var messageId = Guid.NewGuid().ToString();
                cmd.CommandText = $"INSERT INTO public.dialogs (FromUserId, ToUserId, Text, CreatedAt, IsRead, MessageId)" +
                                  $" VALUES (@FromUserId, @ToUserId, @Text, @CreatedAt, @IsRead, @MessageId) ";
                cmd.Parameters.AddWithValue("FromUserId", request.FromUserId);
                cmd.Parameters.AddWithValue("ToUserId", request.ToUserId);
                cmd.Parameters.AddWithValue("Text", request.Text);
                cmd.Parameters.AddWithValue("CreatedAt", messageCreatedAt);
                cmd.Parameters.AddWithValue("IsRead", false);
                cmd.Parameters.AddWithValue("MessageId", messageId);


                await cmd.ExecuteNonQueryAsync(cancellationToken);

                return messageId;
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
