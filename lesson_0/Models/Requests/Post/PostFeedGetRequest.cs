using MediatR;

namespace lesson_0.Models.Requests.Post
{
    public class PostFeedGetRequest : IRequest<PostModel[]>
    {
        /// <summary>Идентификатор пользователя</summary>
        public Guid UserId { get; set; }
        public int Offset { get; set; }
        public int Limit { get; set; }
    }
}