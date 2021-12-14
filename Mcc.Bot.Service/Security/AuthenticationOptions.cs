namespace Mcc.Bot.Service.Security;

internal class AuthenticationOptions
{
    public string SigningKey { get; set; } = string.Empty;
    public string FirstSecret { get; set; } = string.Empty;
}
