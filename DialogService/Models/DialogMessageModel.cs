﻿using System.Text.Json.Serialization;

namespace Dialogs.Models
{
    public class DialogMessageModel
    {
        /// <summary>Текст сообщения</summary>
        /// <example>Привет, как дела?</example>
        [JsonPropertyName("text")]
        public required string Text { get; set; }

        /// <summary>Идентификатор пользователя-отправителя</summary>
        /// <example>dd5feafb-166e-48de-a3cf-bb115b3827e9</example>
        [JsonPropertyName("from")]
        public required string FromUserId { get; set; }

        /// <summary>Идентификатор пользователя-получателя</summary>
        /// <example>dd5feafb-166e-48de-a3cf-bb115b3827e9</example>
        [JsonPropertyName("to")]
        public required string ToUserId { get; set; }

        /// <summary>Время создания сообщения</summary>
        [JsonIgnore]
        public long CreatedAt { get; set; }
    }
}