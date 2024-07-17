using MediatR;

namespace lesson_0.Models.Requests
{
    public class LoginRequest : IRequest<LoginResponse>
    {
        /// <summary>Логин</summary>
        /// <example>otus</example>
        public required string UserName { get; set; }
        /// <summary>Пароль</summary>
        /// <example>Секретная строка</example>
        public required string Password { get; set; }
    }
}