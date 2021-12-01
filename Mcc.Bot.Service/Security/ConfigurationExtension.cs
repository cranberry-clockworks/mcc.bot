using Microsoft.Extensions.Configuration;

namespace Mcc.Bot.Service.Security;

public static class ConfigurationExtension
{
    public static string GetAuthenticationSigningKey(this IConfiguration self)
        => self.GetSection("Authentication").GetValue<string>("SigningKey");
}
