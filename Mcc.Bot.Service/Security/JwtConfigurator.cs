namespace Mcc.Bot.Service.Security;

/// <summary>
/// Settings for generating JSON web tokens.
/// </summary>
internal static class JwtConfigurator
{
    /// <summary>
    /// The issuer of the JSON web token.
    /// </summary>
    public const string Issuer = "Mcc.Bot.Service";

    /// <summary>
    /// The audience of the JSON web token.
    /// </summary>
    public const string Audience = Issuer;
}
