using Mcc.Bot.Service.Models;
using System.Threading.Tasks;

namespace Mcc.Bot.Service.Data;

/// <summary>
/// A storage to hold authentication tokens for users.
/// </summary>
public interface ITokenStorage
{
    /// <summary>
    /// Look ups the token in the storage by given secret. If found the token is removed from the
    /// storage and is returned to the user.
    /// </summary>
    /// <param name="secret">
    /// A secret used to find proper authentication token.
    /// </param>
    /// <returns>
    /// An authentication token. If the token not found by secret the method returns
    /// <see langword="null"/>.
    /// </returns>
    Task<AuthenticationToken?> ConsumeAuthenticationTokenAsync(string secret);

    /// <summary>
    /// Store a new authentication token that can be later used to perform authentication.
    /// </summary>
    /// <param name="token">
    /// A token to store.
    /// </param>
    Task StoreAuthenticationTokenAsync(AuthenticationToken token);
}

/// <summary>
/// Implementation of the <see cref="ITokenStorage"/>.
/// </summary>
internal class TokenStorage : ITokenStorage
{
    private readonly ServiceContext context;

    /// <summary>
    /// Creates token storage.
    /// </summary>
    public TokenStorage(ServiceContext context)
    {
        this.context = context;
    }

    public async Task<AuthenticationToken?> ConsumeAuthenticationTokenAsync(string secret)
    {
        var token = await context.AuthenticationTokens.FindAsync(secret);
        if (token is null)
            return null;

        context.AuthenticationTokens.Remove(token);
        await context.SaveChangesAsync();

        return token;
    }

    public async Task StoreAuthenticationTokenAsync(AuthenticationToken token)
    {
        context.Add(token);
        await context.SaveChangesAsync();
    }
}