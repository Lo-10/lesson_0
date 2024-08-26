using MediatR;

namespace lesson_0.Models.Requests.Post
{
    public class PostFeedUpdateNotification : INotification
    {
        /// <summary>Пост пользователя</summary>
        public required PostModel Post { get; set; }
    }
}