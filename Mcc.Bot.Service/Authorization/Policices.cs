using Mcc.Bot.Service.Models;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Security.Claims;

namespace Mcc.Bot.Service.Authorization;

internal static class Policices
{
    public const string CanManageVacanciesPolicy = canManageVacanciesClaimName;

    public static readonly Action<AuthorizationPolicyBuilder> CanManageVacanciesPolicyBuilder
        = policy => policy.RequireClaim(canManageVacanciesClaimName, true.ToString());

    private const string userIdClaimName = nameof(Identity.UserId);
    private const string canManageVacanciesClaimName = nameof(Identity.CanManageVacancies);

    public static ClaimsIdentity From(Identity identity)
    {
        return new (
            new []
            {
                new Claim(
                    userIdClaimName,
                    identity.UserId.ToString()),
                new Claim(
                    canManageVacanciesClaimName,
                    identity.CanManageVacancies.ToString())
            },
            "Token"
        );
    }
}
