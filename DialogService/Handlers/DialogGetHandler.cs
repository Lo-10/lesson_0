namespace Dialogs.Handlers
{
    using Autofac;
    using Dialogs.Models;
    using MediatR;
    using Npgsql;

    public partial class DialogGetHandler : IRequestHandler<DialogGetRequest, DialogMessage[]>
    {
        private readonly NpgsqlDataSource _dataSource;
        private readonly ILogger<DialogGetHandler> _logger;
        public DialogGetHandler(ILifetimeScope scope)
        {
            _dataSource = scope.Resolve<ReadDataSource>().DataSource;
            _logger = scope.Resolve<ILogger<DialogGetHandler>>();
        }

        public async Task<DialogMessage[]> Handle(DialogGetRequest request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("{HandlerName}:{RequestId} Enter with request {@Request}", GetType().Name, request.RequestId, request);
            try
            {
                await using var cmd = _dataSource.CreateCommand();

                cmd.CommandText = $"SELECT * FROM public.dialogs " +
                                  $"WHERE FromUserId = '{request.FromUserId}' AND ToUserId = '{request.ToUserId}'";

                await using NpgsqlDataReader reader = await cmd.ExecuteReaderAsync(cancellationToken);

                var result = new List<DialogMessage>();

                while (await reader.ReadAsync())
                {
                    result.Add(new DialogMessage()
                    {
                        FromUserId = request.FromUserId,
                        ToUserId = request.ToUserId,
                        Text = (string)reader["Text"],
                        CreatedAt = reader["CreatedAt"].ToString()
                    });
                }

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
