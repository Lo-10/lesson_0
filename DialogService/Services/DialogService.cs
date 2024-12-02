using Grpc.Core;
using Dialogs;
using MediatR;
using Autofac;

namespace Dialogs.Services
{
    public class DialogService : Dialogs.DialogService.DialogServiceBase
    {
        private readonly ILogger<DialogService> _logger;
        private readonly IMediator _mediator;
        public DialogService(ILifetimeScope scope)
        {
            _logger = scope.Resolve<ILogger<DialogService>>();
            _mediator = scope.Resolve<IMediator>();
        }

        public override async Task<SendMessageReply> MessageSend(SendMessageRequest request, ServerCallContext context)
        {
            _logger.LogInformation("{ServiceName}:{RequestId} Enter with request {@Request}", GetType().Name, request.RequestId, request);
            var res = await _mediator.Send(new Models.SendMessageRequest()
            {
                FromUserId = request.FromUserId,
                ToUserId = request.ToUserId,
                Text = request.Text,
                RequestId = request.RequestId
            });

            _logger.LogInformation("{ServiceName}:{RequestId} Exit", GetType().Name, request.RequestId);

            return new SendMessageReply
            {
                MessageId = res
            };
        }

        public override async Task<DialogGetReply> DialogGet(DialogGetRequest request, ServerCallContext context)
        {
            _logger.LogInformation("{ServiceName}:{RequestId} Enter with request {@Request}", GetType().Name, request.RequestId, request);
            var res = await _mediator.Send(new Models.DialogGetRequest()
            {
                FromUserId = request.FromUserId,
                ToUserId = request.ToUserId,
                RequestId = request.RequestId
            });

            var r = new DialogGetReply();

            r.Result.AddRange(res);

            _logger.LogInformation("{ServiceName}:{RequestId} Exit", GetType().Name, request.RequestId);

            return r;
        }
    }
}
