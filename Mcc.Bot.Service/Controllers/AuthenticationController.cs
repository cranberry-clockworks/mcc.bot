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

[Route("[controller]")]
[ApiController]
public class AuthenticationController : ControllerBase
{
    private readonly ILogger<AuthenticationController> logger;
    private readonly ITokenStorage tokenStorage;
    private readonly ISecretGenerator secretGenerator;
    private readonly SigningCredentials signingCredentials;

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
            keychain.SecurityKey,
            SecurityAlgorithms.HmacSha512
        );
    }

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
            issuer: AuthenticationOptions.TokenIssuer,
            audience: AuthenticationOptions.TokenAudience,
            claims: identity.Claims,
            signingCredentials: signingCredentials
        );

        var encoded = new JwtSecurityTokenHandler().WriteToken(jwt);
        return Ok(new
        {
            UserId = userId,
            Token = encoded
        });
    }

    [HttpPost("Secret")]
    [Authorize(Policy = Policices.CanManagePermissionsPolicy)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<string>> EmitTokenAsync(
        bool canManageVacancies = false,
        bool canManagePermissions = false
    )
    {
        if (canManagePermissions == false && canManagePermissions == false)
            return BadRequest("Please pass one of the attributes.");

        var secret = secretGenerator.GenerateSecret();
        var token = new AuthenticationToken()
        {
            Secret = secret,
            CanManageVacancies = canManageVacancies,
            CanManagePermissions = canManagePermissions
        };

        await tokenStorage.EmitNewAuthenticationToken(token);
        return Ok(secret);
    }
}
