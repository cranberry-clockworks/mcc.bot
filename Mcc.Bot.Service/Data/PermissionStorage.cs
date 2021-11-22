using System;
using System.Linq;
using System.Threading.Tasks;
using Mcc.Bot.Service.Models;
using Microsoft.EntityFrameworkCore;

namespace Mcc.Bot.Service.Data;

public interface IPermissionStorage
{
    public Task<bool> CanManagePermissions(ulong userId);
    public Task<bool> CanManageVacancies(ulong userId);

    public Task GrantAllPermissions(ulong userId);
    public Task RevokePermissionToManagePermissions(ulong userId);
    public Task GrantPermissionToManageVacancies(ulong userId);
    public Task RevokePermissionToManageVacancies(ulong userId);
}

public class PermissionStorage : IPermissionStorage
{
    private readonly ServiceContext context;

    public PermissionStorage(ServiceContext context)
    {
        this.context = context;
    }

    public Task<bool> CanManagePermissions(ulong userId)
        => context.Permissions
            .AsNoTracking()
            .Where(p => p.UserId == userId)
            .Select(p => p.CanManagePermissions)
            .FirstOrDefaultAsync();

    public Task<bool> CanManageVacancies(ulong userId)
        => context.Permissions
                .AsNoTracking()
                .Where(p => p.UserId == userId)
                .Select(p => p.CanManageVacancies)
                .FirstOrDefaultAsync();

    public Task GrantAllPermissions(ulong userId)
        => GrantPermissionAndCreateRecordIfNeeded(
            userId,
            p => p.CanManagePermissions = true,
            () => new Permission
            {
                CanManagePermissions = true,
                CanManageVacancies = true
            }
        );

    public Task RevokePermissionToManagePermissions(ulong userId)
        => RevokePermissionAndDeleteRecordIfNeeded(
            userId,
            p => p.CanManagePermissions = false
        );

    public Task GrantPermissionToManageVacancies(ulong userId)
        => GrantPermissionAndCreateRecordIfNeeded(
            userId,
            p => p.CanManageVacancies = true,
            () => new Permission { CanManageVacancies = true }
        );

    public Task RevokePermissionToManageVacancies(ulong userId)
        => RevokePermissionAndDeleteRecordIfNeeded(
            userId,
            p => p.CanManageVacancies = false
        );

    private async Task GrantPermissionAndCreateRecordIfNeeded(
        ulong userId,
        Action<Permission> mutator,
        Func<Permission> permissionFactory
    )
    {
        var permission = await context.Permissions.FindAsync(userId);

        if (permission != null)
        {
            mutator(permission);
        }
        else
        {
            var p = permissionFactory();
            context.Permissions.Add(p);
        }

        await context.SaveChangesAsync();
    }

    private async Task RevokePermissionAndDeleteRecordIfNeeded(
        ulong userId,
        Action<Permission> mutator
    )
    {
        var permission = await context.Permissions.FindAsync(userId);

        if (permission == null)
            return;

        mutator(permission);

        if (permission.CanBeDeleted)
            context.Permissions.Remove(permission);

        await context.SaveChangesAsync();
    }
}