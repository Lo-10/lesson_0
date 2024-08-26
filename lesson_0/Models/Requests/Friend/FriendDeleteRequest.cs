using MediatR;

namespace lesson_0.Models.Requests.Friend
{
    public class FriendDeleteRequest : IRequest<bool?>
    {
        /// <summary>Идентификатор пользователя</summary>
        public string UserId { get; set; }

        /// <summary>Идентификатор пользователя-друга</summary>
        public string FriendId { get; set; }
    }
}