using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace BBCFinanceAPI.Auth;

public class AuthOptions
{
    public const string ISSUER = "MyAuthServer";
    public const string AUDIENCE = "MyAuthClientBot";
    private const string KEY = "secret";
    public static SymmetricSecurityKey GetSymmetricSecurityKey() => 
        new SymmetricSecurityKey(Encoding.UTF8.GetBytes(KEY));
}