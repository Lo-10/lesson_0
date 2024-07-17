namespace lesson_0.Models
{
    public class UserModel
    {
        /// <summary>Идентификатор пользователя</summary>
        /// <example>e4d2e6b0-cde2-42c5-aac3-0b8316f21e58</example>
        public string UserId { get; set; }
        /// <summary>Имя</summary>
        /// <example>Отус</example>
        public string FirstName { get; set; }
        /// <summary>Фамилия</summary>
        /// <example>Отусов</example>
        public string SecondName { get; set; }
        /// <summary>Дата рождения</summary>
        /// <example>2017-02-01</example>
        public DateOnly? BirthDate { get; set; }
        /// <summary>Интересы</summary>
        /// <example>Хобби, интересы и т.п.</example>
        public string Biography { get; set; }
        /// <summary>Город</summary>
        /// <example>Москва</example>
        public string City { get; set; }
    }
}