using Mcc.Bot.Service.Security;

namespace Mcc.Bot.Service.Models;

/// <summary>
/// A token used to authenticate user.
/// </summary>
public class AuthenticationToken
{
    /// <summary>
    /// A special secret string that kept by user and used by him to perform authentication.
    /// </summary>
    public string Secret { get; init; } = string.Empty;

    /// <summary>
    /// If the user is allowed manage permission of another users e.g. emit a new authentication
    /// tokens.
    /// </summary>
    public bool CanManagePermissions { get; set; }

    /// <summary>
    /// If the user is allowed to crate and close vacancies.
    /// </summary>
    public bool CanManageVacancies { get; set; }
}