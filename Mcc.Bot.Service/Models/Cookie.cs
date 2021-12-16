namespace Mcc.Bot.Service.Models;

/// <summary>
/// A cookie that returned to user in authentication process.
/// </summary>
/// <param name="AccessToken">
/// A bearer token used to authorize the user.
/// </param>
/// <param name="UserId">
/// A user id of the bearer.
/// </param>
/// <param name="CanManagePermissions">
/// If the user can manage permissions of other users.
/// </param>
/// <param name="CanManageVacancies">
/// If the user can create and close vacancies.
/// </param>
public record Cookie(
    string AccessToken,
    ulong UserId,
    bool CanManagePermissions,
    bool CanManageVacancies
);