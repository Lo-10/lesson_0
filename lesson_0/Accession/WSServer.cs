using Autofac;
using lesson_0.Models;
using lesson_0.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Saunter.Attributes;
using System.Security.Claims;

namespace lesson_0.Accession
{
    public interface IWSServer
    {
        public Task PostFeedPosted(string userId, PostModel post);
    }

    [AsyncApi]

    public class WSServer : Hub, IWSServer
    {
        private readonly IEventBusService _eventBus;

        public WSServer(ILifetimeScope scope)
        {
            _eventBus = scope.Resolve<IEventBusService>();
        }

        [Channel("/post/feed/posted", Servers = ["prod"], Description = "Канал используется для быстрого обновления ленты постов от друзей пользователя")]
        [SubscribeOperation(typeof(PostModel), Description = "Событие публикации поста одного из друзей пользователя")]
        public async Task PostFeedPosted(string userId, PostModel post)
        {
            try
            {
                await Clients.User(Context.UserIdentifier).SendAsync("ReceiveMessage", post);
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        [Authorize]
        public override async Task OnConnectedAsync()
        {
            _eventBus.Connect(Context.UserIdentifier);

            await Clients.User(Context.UserIdentifier).SendAsync("ReceiveMessage", $"Hi, {Context.UserIdentifier}");
        }
    }
    public class CustomUserIdProvider : IUserIdProvider
    {
        public virtual string? GetUserId(HubConnectionContext connection)
        {
            var identity = connection.User.Identity as ClaimsIdentity;
            return identity.FindFirst("userId")?.Value;
        }
    }
}
