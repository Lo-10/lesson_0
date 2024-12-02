using Autofac;
using Counters;
using Counters.Models;
using Grpc.Core;
using MediatR;

namespace Counters.Services
{
    public class CounterService : Counters.CounterService.CounterServiceBase
    {
        private readonly ILogger<CounterService> _logger;
        private readonly IMediator _mediator;
        public CounterService(ILifetimeScope scope)
        {
            _logger = scope.Resolve<ILogger<CounterService>>();
            _mediator = scope.Resolve<IMediator>();
        }

        public override async Task<MessageCreatedReply> IncreaseUnreadMessageCounter(MessageCreatedRequest request, ServerCallContext context)
        {
            _logger.LogInformation("{ServiceName}:{RequestId} Enter with request {@Request}", GetType().Name, request.RequestId, request);
            var res = await _mediator.Send(new IncreaseUnreadMessageCounterRequest()
            {
                UserId = request.ToUserId,
                RequestId = request.RequestId
            });

            _logger.LogInformation("{ServiceName}:{RequestId} Exit", GetType().Name, request.RequestId);

            return new MessageCreatedReply
            {
                Result = res
            };
        }

        public override async Task<GetUnreadMessageCountReply> GetUnreadMessageCount(GetUnreadMessageCountRequest request, ServerCallContext context)
        {
            _logger.LogInformation("{ServiceName}:{RequestId} Enter with request {@Request}", GetType().Name, request.RequestId, request);
            var res = await _mediator.Send(new Models.GetUnreadMessageCountRequest()
            {
                UserId = request.UserId,
                RequestId = request.RequestId
            });

            _logger.LogInformation("{ServiceName}:{RequestId} Exit", GetType().Name, request.RequestId);

            return new GetUnreadMessageCountReply
            {
                Result = res ?? -1
            };
        }
    }
}
