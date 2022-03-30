using Mcc.Bot.Service.Security;
using Microsoft.IdentityModel.Tokens;
using Moq;
using System.Text;

namespace Mcc.Bot.Service.Tests.Mocks;

internal static class KeychainMock
{
    public static Mock<IKeychain> CreateWithKey(string key)
    {
        var signingKey = new SymmetricSecurityKey(Encoding.Unicode.GetBytes(key));

        var mock = new Mock<IKeychain>(MockBehavior.Strict);
        mock.Setup(x => x.SigningKey).Returns(signingKey);

        return mock;
    }
}
