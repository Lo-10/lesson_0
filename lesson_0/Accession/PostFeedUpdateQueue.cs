using System.Threading.Channels;

namespace lesson_0.Accession
{
    public interface IBackgroundTaskQueue
    {
        ValueTask QueuePostFeedUpdateTaskAsync(
            Func<CancellationToken, Task> workItem);

        ValueTask<Func<CancellationToken, Task>> DequeueAsync(
            CancellationToken cancellationToken);
    }
    public sealed class PostFeedUpdateQueue : IBackgroundTaskQueue
    {
        private readonly Channel<Func<CancellationToken, Task>> _queue;

        public PostFeedUpdateQueue(int capacity)
        {
            BoundedChannelOptions options = new(capacity)
            {
                FullMode = BoundedChannelFullMode.Wait
            };
            _queue = Channel.CreateBounded<Func<CancellationToken, Task>>(options);
        }

        public async ValueTask QueuePostFeedUpdateTaskAsync(
            Func<CancellationToken, Task> workItem)
        {
            ArgumentNullException.ThrowIfNull(workItem);

            await _queue.Writer.WriteAsync(workItem);
        }

        public async ValueTask<Func<CancellationToken, Task>> DequeueAsync(
            CancellationToken cancellationToken)
        {
            Func<CancellationToken, Task>? workItem =
                await _queue.Reader.ReadAsync(cancellationToken);

            return workItem;
        }
    }
}
