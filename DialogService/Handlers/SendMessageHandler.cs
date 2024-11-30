namespace Dialogs.Handlers
{
    using Autofac;
    using MediatR;
    using Npgsql;
    using Dialogs.Models;

    public partial class SendMessageHandler : IRequestHandler<SendMessageRequest, bool?>
    {
        private readonly NpgsqlDataSource _dataSource;
        private readonly ILogger<SendMessageHandler> _logger;

        public SendMessageHandler(ILifetimeScope scope)
        {
            _dataSource = scope.Resolve<WriteDataSource>().DataSource;
            _logger = scope.Resolve<ILogger<SendMessageHandler>>();
        }

        public async Task<bool?> Handle(SendMessageRequest request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("{HandlerName}:{RequestId} Enter with request {@Request}", GetType().Name, request.RequestId, request);
            try
            {
                await using var cmd = _dataSource.CreateCommand();

                var messageCreatedAt = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();
                cmd.CommandText = $"INSERT INTO public.dialogs (FromUserId, ToUserId, Text, CreatedAt)" +
                                  $" VALUES (@FromUserId, @ToUserId, @Text, @CreatedAt)";
                cmd.Parameters.AddWithValue("FromUserId", request.FromUserId);
                cmd.Parameters.AddWithValue("ToUserId", request.ToUserId);
                cmd.Parameters.AddWithValue("Text", request.Text);
                cmd.Parameters.AddWithValue("CreatedAt", messageCreatedAt);

                await cmd.ExecuteNonQueryAsync(cancellationToken);

                return true;
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
