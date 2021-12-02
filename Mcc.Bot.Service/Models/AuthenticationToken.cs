using Mcc.Bot.Service.Security;

namespace Mcc.Bot.Service.Models;

public class AuthenticationToken
{
    public string Secret { get; init; } = string.Empty;

    public bool CanManagePermissions { get; set; }
    public bool CanManageVacancies { get; set; }
}