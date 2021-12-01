using System.Security.Claims;

namespace Mcc.Bot.Service.Models;

public class Identity
{
    public ulong UserId { get; init; }

    public bool CanManagePermissions { get; set; }
    public bool CanManageVacancies { get; set; }

    public bool CanBeDeleted => !(CanManagePermissions || CanManageVacancies);
}