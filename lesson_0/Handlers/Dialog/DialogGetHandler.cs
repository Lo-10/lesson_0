namespace lesson_0.Handlers
{
    using Autofac;
    using lesson_0.Accession;
    using lesson_0.Models;
    using lesson_0.Models.Requests.Dialog;
    using lesson_0.Models.Requests.Post;
    using MediatR;
    using Npgsql;

    public partial class DialogGetHandler : IRequestHandler<DialogGetRequest, DialogMessageModel[]>
    {
        private readonly IRedisCacheProvider _cache;
        public DialogGetHandler(ILifetimeScope scope)
        {
            _cache = scope.Resolve<IRedisCacheProvider>();
        }

        public async Task<DialogMessageModel[]> Handle(DialogGetRequest request, CancellationToken cancellationToken)
        {
            try
            {
                List<DialogMessageModel> result = [];

                var res = await _cache.GetDialogAsync(request.FromUserId.ToString(), request.ToUserId.ToString());
                for (int i = 0; i < res?.Length; i++)
                {
                    for (int j = 0; j < res[i][1].Length; j++)
                    {
                        {
                            result.Add(new DialogMessageModel()
                            {
                                FromUserId = request.FromUserId,
                                ToUserId = request.ToUserId,
                                Text = res[i][1][j][1][3].ToString()
                            });
                        }
                    }
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
