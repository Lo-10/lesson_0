using MediatR;

namespace Counters.Models
{
    public class IncreaseUnreadMessageCounterRequest : IRequest<bool>
    {
        /// <summary>Идентификатор пользователя-получателя</summary>
        public string UserId { get; set; }

        /// <summary>Идентификатор запроса (x-request-id)</summary>
        public string RequestId { get; set; }
    }
}