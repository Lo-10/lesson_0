using lesson_0.Models.Requests;
using MediatR;

namespace lesson_0.Models
{
    public class UserGetRequest : IRequest<UserModel>
    {
        /// <summary>Идентификатор пользователя</summary>
        /// <example>e4d2e6b0-cde2-42c5-aac3-0b8316f21e58</example>
        public string UserId { get; set; }
    }
}