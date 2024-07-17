using MediatR;

namespace lesson_0.Models.Requests
{
    public class UserRegisterRequest: IRequest<UserRegisterResponse>
    {
        /// <summary>Логин</summary>
        /// <example>otus</example>
        public required string UserName { get; set; }
        /// <summary>Имя</summary>
        /// <example>Отус</example>
        public required string FirstName { get; set; }
        /// <summary>Фамилия</summary>
        /// <example>Отусов</example>
        public required string SecondName { get; set; }
        /// <summary>Дата рождения</summary>
        /// <example>2017-02-01</example>
        public required DateOnly BirthDate { get; set; }
        /// <summary>Интересы</summary>
        /// <example>Хобби, интересы и т.п.</example>
        public required string Biography { get; set; }
        /// <summary>Город</summary>
        /// <example>Москва</example>
        public required string City { get; set; }
        /// <summary>Пароль</summary>
        /// <example>Секретная строка</example>
        public required string Password { get; set; }
    }
}