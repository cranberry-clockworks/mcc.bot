using Mcc.Bot.Service.Models;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Security.Claims;
using System.Security.Principal;

namespace Mcc.Bot.Service.Security;

internal static class Policices
{
    public const string CanManageVacanciesPolicy = canManageVacanciesClaimName;
    public const string CanManagePermissionsPolicy = canManagePermissionsClaimName;

    public static readonly Action<AuthorizationPolicyBuilder> CanManageVacanciesPolicyBuilder
        = policy => policy.RequireClaim(canManageVacanciesClaimName, true.ToString());

    public static readonly Action<AuthorizationPolicyBuilder> CanManagePermissionsPolicyBuilder
        = policy => policy.RequireClaim(canManagePermissionsClaimName, true.ToString());

    private const string canManageVacanciesClaimName
        = nameof(AuthenticationToken.CanManageVacancies);

    private const string canManagePermissionsClaimName
        = nameof(AuthenticationToken.CanManagePermissions);

    public static ClaimsIdentity CreateClaimIdentity(ulong userId, AuthenticationToken token)
    {
        return new ClaimsIdentity(
            new []
            {
                new Claim(
                    ClaimTypes.Name,
                    userId.ToString()
                ),
                new Claim(
                    canManageVacanciesClaimName,
                    token.CanManageVacancies.ToString()
                ),
                new Claim(
                    canManagePermissionsClaimName,
                    token.CanManagePermissions.ToString()
                )
            },
            "Token"
        );
    }

    public static ulong? GetUserId(this IIdentity self)
    {
        var name = self.Name;
        return name is not null
            ? ulong.TryParse(name, out var userId) ? userId : null
            : null;
    }
}
