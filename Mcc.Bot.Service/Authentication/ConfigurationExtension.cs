using Microsoft.Extensions.Configuration;

namespace Mcc.Bot.Service.Authentication;

public static class ConfigurationExtension
{
    public static string GetAuthenticationSigningKey(this IConfiguration self)
        => self.GetSection("Authentication").GetValue<string>("SigningKey");
}
