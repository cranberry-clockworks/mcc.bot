using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Mcc.Bot.Service.Security;

public interface IKeychain
{
    SecurityKey SecurityKey { get; }
}

internal class Keychain : IKeychain
{
    public SecurityKey SecurityKey { get; }

    public Keychain(string signingKey)
    {
        SecurityKey = new SymmetricSecurityKey(Encoding.Unicode.GetBytes(signingKey));
    }
}
