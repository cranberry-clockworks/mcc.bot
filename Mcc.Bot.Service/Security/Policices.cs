using Mcc.Bot.Service.Models;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Security.Claims;

namespace Mcc.Bot.Service.Security;

internal static class Policices
{
    public const string CanManageVacanciesPolicy = canManageVacanciesClaimName;
    public const string CanManagePermissionsPolicy = canManagePermissionsClaimName;

    public static readonly Action<AuthorizationPolicyBuilder> CanManageVacanciesPolicyBuilder
        = policy => policy.RequireClaim(canManageVacanciesClaimName, true.ToString());

    public static readonly Action<AuthorizationPolicyBuilder> CanManagePermissionsPolicyBuilder
        = policy => policy.RequireClaim(canManagePermissionsClaimName, true.ToString());

    private const string userIdClaimName = nameof(AuthenticationToken.Secret);

    private const string canManageVacanciesClaimName
        = nameof(AuthenticationToken.CanManageVacancies);

    private const string canManagePermissionsClaimName
        = nameof(AuthenticationToken.CanManagePermissions);

    public static ClaimsIdentity From(AuthenticationToken token)
    {
        return new (
            new []
            {
                new Claim(
                    userIdClaimName,
                    token.Secret.ToString()
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
}
