namespace Mcc.Bot.Service.Security;

/// <summary>
/// Authentication options
/// </summary>
internal class AuthenticationOptions
{
    /// <summary>
    /// A secret key used for security. The length at least of 512 bytes are recommended.
    /// </summary>
    public string SigningKey { get; set; } = string.Empty;
    
    /// <summary>
    /// The first secret id of the secret to obtain the authentication token. Populated into the
    /// database if there is no any secret id.
    /// </summary>
    public string FirstSecret { get; set; } = string.Empty;
}
