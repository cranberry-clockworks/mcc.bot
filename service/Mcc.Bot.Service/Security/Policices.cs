using Mcc.Bot.Service.Models;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Security.Claims;
using System.Security.Principal;

namespace Mcc.Bot.Service.Security;

/// <summary>
/// A class to hold information about authorization policies.
/// </summary>
internal static class Policies
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

    /// <summary>
    /// Creates the claim identity by given parameters.
    /// </summary>
    /// <param name="userId">
    /// An authenticated user id.
    /// </param>
    /// <param name="token">
    /// An authentication token used by user that contains user's privileges.
    /// </param>
    /// <returns>The claim identity.</returns>
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

    /// <summary>
    /// Gets id of the user.
    /// </summary>
    /// <returns>
    /// The id of the user if present in the identity, <see langword="null"/> otherwise.
    /// </returns>
    public static ulong? GetUserId(this IIdentity self)
    {
        var name = self.Name;
        return name is not null
            ? ulong.TryParse(name, out var userId) ? userId : null
            : null;
    }
}
