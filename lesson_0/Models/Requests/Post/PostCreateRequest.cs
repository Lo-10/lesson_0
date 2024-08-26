using MediatR;
using System.Text.Json.Serialization;

namespace lesson_0.Models.Requests.Post
{
    public class PostCreateRequest : IRequest<PostModel>
    {
        /// <summary>Текст поста</summary>
        /// <example>Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Lectus mauris ultrices eros in cursus turpis massa.</example>
        [JsonPropertyName("text")]
        public required string Text { get; set; }

        /// <summary>Идентификатор пользователя</summary>
        [JsonIgnore]
        public Guid UserId { get; set; }
    }
}