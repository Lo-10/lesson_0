namespace lesson_0.Models.Requests
{
    public class LoginResponse
    {
        /// <summary>Bearer токен</summary>
        /// <example>e4d2e6b0-cde2-42c5-aac3-0b8316f21e58</example>
        public required string Token { get; set; }
    }
}