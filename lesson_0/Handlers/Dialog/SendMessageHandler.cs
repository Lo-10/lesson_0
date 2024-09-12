namespace lesson_0.Handlers
{
    using Autofac;
    using lesson_0.Models;
    using lesson_0.Models.Requests.Dialog;
    using MediatR;
    using Npgsql;

    public partial class SendMessageHandler : IRequestHandler<SendMessageRequest, bool?>
    {
        private readonly NpgsqlDataSource _dataSource;

        public SendMessageHandler(ILifetimeScope scope)
        {
            _dataSource = scope.Resolve<WriteDataSource>().DataSource;
        }

        public async Task<bool?> Handle(SendMessageRequest request, CancellationToken cancellationToken)
        {
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
                return null;
            }
            finally
            {

            }
        }
    }
}
