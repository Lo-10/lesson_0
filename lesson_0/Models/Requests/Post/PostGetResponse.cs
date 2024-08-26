using System.Text.Json.Serialization;

namespace lesson_0.Models.Requests.Post
{
    public class PostGetResponse
    {
        /// <summary>Идентификатор поста</summary>
        [JsonPropertyName("id")]
        public Guid PostId { get; set; }
        /// <summary>Текст поста</summary>
        /// <example>Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Lectus mauris ultrices eros in cursus turpis massa.</example>
        [JsonPropertyName("text")]
        public string Text { get; set; }

        /// <summary>Идентификатор пользователя</summary>
        [JsonPropertyName("author_user_id")]
        public string UserId { get; set; }
    }
}