using Microsoft.Extensions.Configuration;

namespace Mcc.Bot.Service.Security;

/// <summary>
/// Extension method collection for the <see cref="IConfiguration"/>.
/// </summary>
internal static class ConfigurationExtension
{
    /// <summary>
    /// Gets authentication section options.
    /// </summary>
    /// <param name="self">
    /// The configuration instance to obtain the section.
    /// </param>
    /// <returns>
    /// The configuration section.
    /// </returns>
    public static IConfigurationSection GetAuthenticationSection(this IConfiguration self)
        => self.GetSection("Authentication");

    /// <summary>
    /// Gets authentication section options.
    /// </summary>
    /// <param name="self">
    /// The configuration instance to obtain the section.
    /// </param>
    /// <returns>
    /// The configuration section as <see cref="AuthenticationOptions"/>.
    /// </returns>
    public static AuthenticationOptions GetAuthenticationOptions(this IConfiguration self)
        => self.GetAuthenticationSection().Get<AuthenticationOptions>();
}