using Microsoft.Extensions.Options;
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
    SecurityKey SigningKey { get; }
}

/// <summary>
/// An implementation of the <see cref="IKeychain"/>.
/// </summary>
internal class Keychain : IKeychain
{
    public SecurityKey SigningKey { get; }

    public Keychain(IOptions<AuthenticationOptions> options)
    {
        var key = options.Value.SigningKey;
        SigningKey = new SymmetricSecurityKey(Encoding.Unicode.GetBytes(key));
    }
}
