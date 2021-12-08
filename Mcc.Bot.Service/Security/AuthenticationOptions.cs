namespace Mcc.Bot.Service.Security;

internal class AuthenticationOptions
{
    public const string AuthenticationSection = "Authentication";

    public string SigningKey { get; set; } = string.Empty;
    public string FirstSecret { get; set; } = string.Empty;
}
