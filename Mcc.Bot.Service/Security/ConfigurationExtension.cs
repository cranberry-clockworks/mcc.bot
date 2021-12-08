using Microsoft.Extensions.Configuration;

namespace Mcc.Bot.Service.Security;

internal static class ConfigurationExtension
{
    public static string GetAuthenticationSigningKey(this IConfiguration self)
        => self.GetSection("Authentication").GetValue<string>("SigningKey");
}
