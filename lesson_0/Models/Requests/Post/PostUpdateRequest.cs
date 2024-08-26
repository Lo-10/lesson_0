using MediatR;
using System.Text.Json.Serialization;

namespace lesson_0.Models.Requests.Post
{
    public class PostUpdateRequest : IRequest<bool?>
    {
        /// <summary>Идентификатор поста</summary>
        /// <example>dd5feafb-166e-48de-a3cf-bb115b3827e9</example>
        [JsonPropertyName("id")]
        public required Guid PostId { get; set; }
        /// <summary>Текст поста</summary>
        /// <example>Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Lectus mauris ultrices eros in cursus turpis massa.</example>
        public required string Text { get; set; }

        /// <summary>Идентификатор пользователя</summary>
        [JsonIgnore]
        public Guid UserId { get; set; }
    }
}