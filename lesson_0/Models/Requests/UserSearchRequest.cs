using lesson_0.Models.Requests;
using MediatR;

namespace lesson_0.Models
{
    public class UserSearchRequest : IRequest<UserModel>
    {
        /// <summary>Условие поиска по имени</summary>
        /// <example>Иванов</example>
        public string FirstName { get; set; }

        /// <summary>Условие поиска по фамилии</summary>
        /// <example>Иван</example>
        public string LastName { get; set; }
    }
}