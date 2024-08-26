using lesson_0.Accession;

namespace lesson_0.Services
{
    public sealed class QueuedPostFeedUpdateService(
            IBackgroundTaskQueue taskQueue,
            ILogger<QueuedPostFeedUpdateService> logger) : BackgroundService
    {
        protected override Task ExecuteAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation($"{nameof(QueuedPostFeedUpdateService)} is running");

            return ProcessTaskQueueAsync(cancellationToken);
        }

        private async Task ProcessTaskQueueAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    var workItem = await taskQueue.DequeueAsync(cancellationToken);

                    await workItem(cancellationToken);
                }
                catch (OperationCanceledException)
                {

                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error occurred executing task work item.");
                }
            }
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation(
                $"{nameof(QueuedPostFeedUpdateService)} is stopping.");

            await base.StopAsync(cancellationToken);
        }
    }
}
