
using Autofac;
using lesson_0.Models.Requests.Post;
using MediatR;

namespace lesson_0.Services
{
    public class PostFeedLoadService : IHostedService
    {
        private readonly IMediator _mediator;
        public PostFeedLoadService(ILifetimeScope scope)
        {
            _mediator = scope.Resolve<IMediator>();
        }
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await _mediator.Publish(new PostFeedLoadNotification(), cancellationToken);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
