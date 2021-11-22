using System.ComponentModel.DataAnnotations.Schema;

namespace Mcc.Bot.Service.Models;

public class Permission
{
    public ulong UserId;
    public bool CanManagePermissions;
    public bool CanManageVacancies;

    [NotMapped]
    public bool CanBeDeleted => !(CanManagePermissions || CanManageVacancies);
}