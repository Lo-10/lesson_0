using MediatR;

namespace lesson_0.Models.Requests.Post
{
    public class PostGetRequest : IRequest<PostGetResponse>
    {
        /// <summary>Идентификатор поста</summary>
        public required Guid PostId { get; set; }
    }
}