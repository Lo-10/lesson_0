using MediatR;
using System.Text.Json.Serialization;

namespace lesson_0.Models.Requests.Dialog
{
    public class UnreadMessageCountGetRequest : IRequest<int?>
    {
        /// <summary>Идентификатор пользователя</summary>
        [JsonIgnore]
        public Guid UserId { get; set; }

        /// <summary>Идентификатор запроса (x-request-id)</summary>
        [JsonIgnore]
        public string RequestId { get; set; }
    }
}