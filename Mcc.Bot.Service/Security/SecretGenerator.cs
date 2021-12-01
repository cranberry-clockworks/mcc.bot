using System;

namespace Mcc.Bot.Service.Security;

public interface ISecretGenerator
{
    string GenerateSecret();
}

internal class SecretGenerator : ISecretGenerator
{
    public string GenerateSecret()
        => Guid.NewGuid().ToString();
}
