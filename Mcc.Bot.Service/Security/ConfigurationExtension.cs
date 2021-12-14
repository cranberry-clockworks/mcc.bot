using Microsoft.Extensions.Configuration;

namespace Mcc.Bot.Service.Security;

internal static class ConfigurationExtension
{
    public static IConfigurationSection GetAuthenticationSection(this IConfiguration self)
        => self.GetSection("Authentication");

    public static AuthenticationOptions GetAuthenticationOptions(this IConfiguration self)
        => self.GetAuthenticationSection().Get<AuthenticationOptions>();
}