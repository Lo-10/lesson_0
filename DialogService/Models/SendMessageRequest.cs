using MediatR;
using System.Text.Json.Serialization;

namespace Dialogs.Models
{
    public class SendMessageRequest : IRequest<string?>
    {
        /// <summary>Текст сообщения</summary>
        /// <example>Привет, как дела?</example>
        public string Text { get; set; }

        /// <summary>Идентификатор пользователя-отправителя</summary>
        public string FromUserId { get; set; }

        /// <summary>Идентификатор пользователя-получателя</summary>
        public string ToUserId { get; set; }

        /// <summary>Идентификатор запроса (x-request-id)</summary>
        public string RequestId { get; set; }
    }
}