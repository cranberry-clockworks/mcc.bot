namespace Mcc.Bot.Service.Models;

public record Cookie(
    string AccessToken,
    ulong UserId,
    bool CanManagePermissions,
    bool CanManageVacancies);