using MediatR;
using System.Text.Json.Serialization;

namespace lesson_0.Models.Requests.Dialog
{
    public class DialogGetRequest : IRequest<DialogMessageModel[]>
    {
        /// <summary>Идентификатор пользователя-отправителя</summary>
        [JsonIgnore]
        public Guid FromUserId { get; set; }

        /// <summary>Идентификатор пользователя-получателя</summary>
        [JsonIgnore]
        public Guid ToUserId { get; set; }
    }
}