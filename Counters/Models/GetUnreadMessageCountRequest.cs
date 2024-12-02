using MediatR;

namespace Counters.Models
{
    public class GetUnreadMessageCountRequest : IRequest<int?>
    {
        /// <summary>Идентификатор пользователя</summary>
        public string UserId { get; set; }

        /// <summary>Идентификатор запроса (x-request-id)</summary>
        public string RequestId { get; set; }
    }
}