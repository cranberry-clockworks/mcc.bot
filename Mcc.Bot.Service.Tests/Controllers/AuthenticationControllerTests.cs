using Mcc.Bot.Service.Controllers;
using Mcc.Bot.Service.Data;
using Mcc.Bot.Service.Security;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Moq;
using NUnit.Framework;
using System.Text;
using System.Threading.Tasks;

namespace Mcc.Bot.Service.Tests.Controllers;

[TestFixture]
internal class AuthenticationControllerTests
{
    [Test]
    public async Task PassInvalidParametersToEmitToken()
    {
        var loggerMock = new Mock<ILogger<AuthenticationController>>();

        var tokenStorageMock = new Mock<ITokenStorage>();

        var key = new SymmetricSecurityKey(
            Encoding.Unicode.GetBytes("{32E6C9AD-A4DD-405F-8369-6EFBB6316A1C}")
        );

        var keychainMock = new Mock<IKeychain>(MockBehavior.Strict);
        keychainMock
            .Setup(x => x.SigningKey)
            .Returns(key);

        var secretGeneratorMock = new Mock<ISecretGenerator>();

        var controller = new AuthenticationController(
            loggerMock.Object,
            tokenStorageMock.Object,
            keychainMock.Object,
            secretGeneratorMock.Object
        );

        var result = await controller.EmitTokenAsync(false, false);

        Assert.That(
            result.Result,
            Is.TypeOf<BadRequestObjectResult>()
        );
    }
}
