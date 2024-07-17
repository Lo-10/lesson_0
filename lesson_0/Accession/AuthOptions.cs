using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace lesson_0.Accession
{
    public class AuthOptions
    {
        public const string ISSUER = "otus"; // издатель токена
        public const string AUDIENCE = "client"; // потребитель токена
        readonly string KEY = "this is my custom Secret key for authentication"; // ключ для шифрования токена
        public const int LIFETIME = 43200; // время жизни токена
        public AuthOptions() { }
        public SymmetricSecurityKey GetSymmetricSecurityKey()
        {
            return new SymmetricSecurityKey(Encoding.ASCII.GetBytes(KEY));
        }
    }
}
