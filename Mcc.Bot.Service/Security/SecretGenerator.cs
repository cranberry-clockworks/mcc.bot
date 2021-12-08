using System;

namespace Mcc.Bot.Service.Security;

/// <summary>
/// A class that provides secret strings.
/// </summary>
public interface ISecretGenerator
{
    /// <summary>
    /// Gets a new secret.
    /// </summary>
    /// <returns>A secret represented in string.</returns>
    string GenerateSecret();
}

/// <summary>
/// Implementation of the <see cref="ISecretGenerator"/>.
/// </summary>
internal class SecretGenerator : ISecretGenerator
{
    /// <inheritdoc />
    public string GenerateSecret()
        => Guid.NewGuid().ToString();
}
