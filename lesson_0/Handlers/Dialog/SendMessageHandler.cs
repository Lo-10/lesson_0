﻿namespace lesson_0.Handlers
{
    using Autofac;
    using Grpc.Net.Client;
    using MediatR;
    using Npgsql;
    using DialogsClient;
    using lesson_0.Models;
    using SendMessageRequest = Models.Requests.Dialog.SendMessageRequest;

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
                using var channel = GrpcChannel.ForAddress(Environment.GetEnvironmentVariables()["dialogs_grpc_url"].ToString());

                var client = new DialogService.DialogServiceClient(channel);

                var reply = await client.MessageSendAsync(new DialogsClient.SendMessageRequest
                {
                    FromUserId = request.FromUserId.ToString(),
                    ToUserId = request.ToUserId.ToString(),
                    Text = request.Text,
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
