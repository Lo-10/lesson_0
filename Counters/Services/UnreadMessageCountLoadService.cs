using Autofac;
using Counters.Models;
using MediatR;

namespace Counters.Services
{
    public class UnreadMessageCountLoadService : IHostedService
    {
        private readonly IMediator _mediator;
        public UnreadMessageCountLoadService(ILifetimeScope scope)
        {
            _mediator = scope.Resolve<IMediator>();
        }
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await _mediator.Publish(new UnreadMessageCountLoadNotification(), cancellationToken);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
