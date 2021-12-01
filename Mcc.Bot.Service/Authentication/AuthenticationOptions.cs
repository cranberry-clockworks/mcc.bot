using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Mcc.Bot.Service.Authentication;

internal static class AuthenticationOptions
{
    public const string TokenIssuer = "Mcc.Bot.Service";
    public const string TokenAudience = TokenIssuer;

    public static SymmetricSecurityKey BuildSigningKey(string secret)
        => new(Encoding.Unicode.GetBytes(secret));
}
