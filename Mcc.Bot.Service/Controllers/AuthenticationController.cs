using Mcc.Bot.Service.Data;
using Mcc.Bot.Service.Models;
using Mcc.Bot.Service.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Threading.Tasks;

namespace Mcc.Bot.Service.Controllers;

/// <summary>
/// Controller to authenticate clients.
/// </summary>
[Route("[controller]")]
[ApiController]
public class AuthenticationController : ControllerBase
{
    private readonly ILogger<AuthenticationController> logger;
    private readonly ITokenStorage tokenStorage;
    private readonly ISecretGenerator secretGenerator;
    private readonly SigningCredentials signingCredentials;

    /// <summary>
    /// Creates controller instance.
    /// </summary>
    public AuthenticationController(
        ILogger<AuthenticationController> logger,
        ITokenStorage tokenStorage,
        IKeychain keychain,
        ISecretGenerator secretGenerator
    )
    {
        this.logger = logger;
        this.tokenStorage = tokenStorage;
        this.secretGenerator = secretGenerator;

        signingCredentials = new SigningCredentials(
            keychain.SigningKey,
            SecurityAlgorithms.HmacSha512
        );
    }

    /// <summary>
    /// Authenticates the user. In case of success authorization the secret can't be used for
    /// another authentication.
    /// </summary>
    /// <param name="userId">
    /// An unique integer number that identifies the user.
    /// </param>
    /// <param name="secret">
    /// A special string that maps to granted user privileges.
    /// The secret could be emitted by another user with right to manage permissions.
    /// Could be used only once to authenticate.
    /// </param>
    /// <returns>The authentication token with granted permission for the user.</returns>
    [HttpPost("Token")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<Cookie>> AuthenticateAsync(
        [Required][FromForm] ulong userId,
        [Required][FromForm] string secret
    )
    {
        var token = await tokenStorage.ConsumeAuthenticationTokenAsync(secret);
        if (token is null)
        {
            logger.LogWarning("Authentication attempt with not emitted key.");
            return Forbid();
        }

        var identity = Policices.CreateClaimIdentity(userId, token);
        var jwt = new JwtSecurityToken(
            issuer: JwtConfigurator.Issuer,
            audience: JwtConfigurator.Audience,
            claims: identity.Claims,
            signingCredentials: signingCredentials
        );

        var encoded = new JwtSecurityTokenHandler().WriteToken(jwt);
        return Ok(
            new Cookie(
                encoded,
                userId,
                token.CanManagePermissions,
                token.CanManageVacancies
            )
        );
    }

    /// <summary>
    /// Emits a new one-time secret that can be given to another user to authenticate.
    /// </summary>
    /// <param name="canManageVacancies">
    /// If the user could create and close vacancies.
    /// </param>
    /// <param name="canManagePermissions">
    /// If the user could emit another tokens and manage
    /// permission of another users.
    /// </param>
    /// <returns></returns>
    [HttpPost("Secret")]
    [Authorize(Policy = Policices.CanManagePermissionsPolicy)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<string>> EmitTokenAsync(
        bool canManageVacancies = false,
        bool canManagePermissions = false
    )
    {
        if (canManagePermissions == false && canManageVacancies == false)
            return BadRequest("Please pass one of the attributes.");

        var secret = secretGenerator.GenerateSecret();
        var token = new AuthenticationToken()
        {
            Secret = secret,
            CanManageVacancies = canManageVacancies,
            CanManagePermissions = canManagePermissions
        };

        await tokenStorage.StoreAuthenticationTokenAsync(token);
        return Ok(secret);
    }
}
