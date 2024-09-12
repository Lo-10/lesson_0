namespace lesson_0.Handlers
{
    using Autofac;
    using lesson_0.Models;
    using lesson_0.Models.Requests.Dialog;
    using lesson_0.Models.Requests.Post;
    using MediatR;
    using Npgsql;

    public partial class DialogGetHandler : IRequestHandler<DialogGetRequest, DialogMessageModel[]>
    {
        private readonly NpgsqlDataSource _dataSource;
        public DialogGetHandler(ILifetimeScope scope)
        {
            _dataSource = scope.Resolve<ReadDataSource>().DataSource;
        }

        public async Task<DialogMessageModel[]> Handle(DialogGetRequest request, CancellationToken cancellationToken)
        {
            try
            {
                await using var cmd = _dataSource.CreateCommand();

                cmd.CommandText = $"SELECT * FROM public.dialogs " +
                                  $"WHERE FromUserId = '{request.FromUserId}' AND ToUserId = '{request.ToUserId}'";

                await using NpgsqlDataReader reader = await cmd.ExecuteReaderAsync(cancellationToken);

                var result = new List<DialogMessageModel>();

                while (await reader.ReadAsync())
                {
                    result.Add(new DialogMessageModel()
                    {
                        FromUserId = request.FromUserId,
                        ToUserId = request.ToUserId,
                        Text = (string)reader["Text"],
                        CreatedAt = (long)reader["CreatedAt"]
                    });
                }

                return result.ToArray();
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
