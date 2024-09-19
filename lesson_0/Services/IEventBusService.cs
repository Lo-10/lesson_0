namespace lesson_0.Services
{
    public interface IEventBusService
    {
        public void Connect(string userId);

        public Task SendMessageAsync(string userId, object message, CancellationToken cancellationToken);
    }
}
