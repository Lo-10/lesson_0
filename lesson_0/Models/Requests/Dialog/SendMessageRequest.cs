﻿using MediatR;
using System.Text.Json.Serialization;

namespace lesson_0.Models.Requests.Dialog
{
    public class SendMessageRequest : IRequest<bool?>
    {
        /// <summary>Текст сообщения</summary>
        /// <example>Привет, как дела?</example>
        [JsonPropertyName("text")]
        public required string Text { get; set; }

        /// <summary>Идентификатор пользователя-отправителя</summary>
        [JsonIgnore]
        public Guid FromUserId { get; set; }

        /// <summary>Идентификатор пользователя-получателя</summary>
        [JsonIgnore]
        public Guid ToUserId { get; set; }
    }
}