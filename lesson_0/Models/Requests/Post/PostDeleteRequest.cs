using MediatR;

namespace lesson_0.Models.Requests.Post
{
    public class PostDeleteRequest : IRequest<bool?>
    {
        /// <summary>Идентификатор поста</summary>
        public required Guid PostId { get; set; }
    }
}