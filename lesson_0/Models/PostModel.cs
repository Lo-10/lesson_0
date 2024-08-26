namespace lesson_0.Models
{
    public class PostModel
    {
        /// <summary>Идентификатор поста</summary>
        public Guid PostId { get; set; }

        /// <summary>Текст поста</summary>
        public string Text { get; set; }

        /// <summary>Идентификатор пользователя</summary>
        public Guid UserId { get; set; }

        /// <summary>Время создания поста</summary>
        public DateTime PostCreatedAt { get; set; }

        /// <summary>Время обновления поста</summary>
        public DateTime PostUpdatedAt { get; set; }
    }
}