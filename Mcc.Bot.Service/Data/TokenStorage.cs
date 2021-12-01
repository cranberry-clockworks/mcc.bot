using Mcc.Bot.Service.Models;
using System.Threading.Tasks;

namespace Mcc.Bot.Service.Data;

public interface ITokenStorage
{
    ValueTask<AuthenticationToken?> ConsumeAuthenticationTokenAsync(string secret);
    ValueTask EmitNewAuthenticationToken(AuthenticationToken token);
}

public class TokenStorage : ITokenStorage
{
    private readonly ServiceContext context;

    public TokenStorage(ServiceContext context)
    {
        this.context = context;
    }

    public async ValueTask<AuthenticationToken?> ConsumeAuthenticationTokenAsync(string secret)
    {
        var token = await context.AuthenticationTokens.FindAsync(secret);
        if (token is null)
            return null;

        context.Remove(token);
        await context.SaveChangesAsync();

        return token;
    }

    public async ValueTask EmitNewAuthenticationToken(AuthenticationToken token)
    {
        context.Add(token);
        await context.SaveChangesAsync();
    }
}