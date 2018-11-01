using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace StatosphericBackend.Options
{
    public class AuthOptions
    {
        public const string Issuer = "StratoAuthServer"; // издатель токена
        public const string Audience = "http://localhost:5000/"; // потребитель токена
        public const string Key = "mysupersecret_secretkey!123";
        public const int Lifetime = 60;

        public static SymmetricSecurityKey GetSymmetricSecurityKey()
        {
            return new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Key));
        }
    }
}