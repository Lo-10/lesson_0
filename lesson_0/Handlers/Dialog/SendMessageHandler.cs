namespace lesson_0.Handlers
{
    using Autofac;
    using lesson_0.Accession;
    using lesson_0.Models;
    using lesson_0.Models.Requests.Dialog;
    using MediatR;
    using Npgsql;

    public partial class SendMessageHandler : IRequestHandler<SendMessageRequest, bool?>
    {
        private readonly IRedisCacheProvider _cache;

        public SendMessageHandler(ILifetimeScope scope)
        {
            _cache = scope.Resolve<IRedisCacheProvider>();
        }

        public async Task<bool?> Handle(SendMessageRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var messageCreatedAt = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();

                var result = await _cache.SendMessageAsync(request.FromUserId.ToString(), request.ToUserId.ToString(), messageCreatedAt, request.Text);

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
