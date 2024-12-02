namespace lesson_0.Handlers
{
    using Autofac;
    using Grpc.Net.Client;
    using lesson_0.Models;
    using lesson_0.Models.Requests.Dialog;
    using lesson_0.Models.Requests.Post;
    using MediatR;
    using Npgsql;
    using System.Linq;
    using DialogsClient;

    public partial class DialogGetHandler : IRequestHandler<Models.Requests.Dialog.DialogGetRequest, DialogMessageModel[]>
    {
        private readonly NpgsqlDataSource _dataSource;
        private readonly ILogger<DialogGetHandler> _logger;
        public DialogGetHandler(ILifetimeScope scope)
        {
            _dataSource = scope.Resolve<ReadDataSource>().DataSource;
            _logger = scope.Resolve<ILogger<DialogGetHandler>>();
        }

        public async Task<DialogMessageModel[]> Handle(Models.Requests.Dialog.DialogGetRequest request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("{HandlerName}:{RequestId} Enter with request {@Request}", GetType().Name, request.RequestId, request);
            try
            {
                using var channel = GrpcChannel.ForAddress(Environment.GetEnvironmentVariables()["dialogs_grpc_url"].ToString());

                var client = new DialogService.DialogServiceClient(channel);

                var reply = await client.DialogGetAsync(new DialogsClient.DialogGetRequest
                {
                    FromUserId = request.FromUserId.ToString(),
                    ToUserId = request.ToUserId.ToString(),
                    RequestId = request.RequestId
                });

                var result = reply.Result.Select(m =>
                    new DialogMessageModel()
                    {
                        FromUserId = Guid.Parse(m.FromUserId),
                        ToUserId = Guid.Parse(m.ToUserId),
                        Text = m.Text,
                        CreatedAt = Int64.Parse(m.CreatedAt)
                    });

                return result.ToArray();
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
