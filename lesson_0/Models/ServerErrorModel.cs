namespace lesson_0.Models
{
    public class ServerErrorModel
    {
        /// <summary>Описание ошибки</summary>
        public required string Message { get; set; }

        /// <summary>Идентификатор запроса. Предназначен для более быстрого поиска проблем.</summary>
        public string? RequestId { get; set; }

        /// <summary>Код ошибки. Предназначен для классификации проблем и более быстрого решения проблем.</summary>
        public int Code { get; set; }
    }
}