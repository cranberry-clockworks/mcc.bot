using System.ComponentModel.DataAnnotations.Schema;

namespace Mcc.Bot.Service.Models;

public class Permission
{
    public ulong UserId { get; init; }
    public bool CanManagePermissions { get; set; }
    public bool CanManageVacancies { get; set; }

    public bool CanBeDeleted => !(CanManagePermissions || CanManageVacancies);
}