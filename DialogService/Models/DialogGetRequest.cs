using MediatR;
using System.Text.Json.Serialization;

namespace Dialogs.Models
{
    public class DialogGetRequest : IRequest<DialogMessage[]>
    {
        /// <summary>Идентификатор пользователя-отправителя</summary>
        public string FromUserId { get; set; }

        /// <summary>Идентификатор пользователя-получателя</summary>
        public string ToUserId { get; set; }

        /// <summary>Идентификатор запроса (x-request-id)</summary>
        public string RequestId { get; set; }
    }
}