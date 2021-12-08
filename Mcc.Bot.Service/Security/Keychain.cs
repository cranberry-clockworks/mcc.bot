using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Mcc.Bot.Service.Security;

/// <summary>
/// A class that provides access to security keys.
/// </summary>
public interface IKeychain
{
    /// <summary>
    /// Gets an common service security key.
    /// </summary>
    SecurityKey SecurityKey { get; }
}

/// <summary>
/// An implementation of the <see cref="IKeychain"/>.
/// </summary>
internal class Keychain : IKeychain
{
    public SecurityKey SecurityKey { get; }

    public Keychain(string signingKey)
    {
        SecurityKey = new SymmetricSecurityKey(Encoding.Unicode.GetBytes(signingKey));
    }
}
