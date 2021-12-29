using Mcc.Bot.Service.Controllers;
using Mcc.Bot.Service.Data;
using Mcc.Bot.Service.Models;
using Mcc.Bot.Service.Security;
using Mcc.Bot.Service.Tests.Mocks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System;
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
        var keychainMock = KeychainMock.CreateWithKey("{32E6C9AD-A4DD-405F-8369-6EFBB6316A1C}");
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

    [Test]
    [TestCase(true, false)]
    [TestCase(false, true)]
    [TestCase(true, true)]
    public async Task EmitNewToken(bool canManageVacancies, bool canManagePermissions)
    {
        var loggerMock = new Mock<ILogger<AuthenticationController>>();

        var tokenStorageMock = new Mock<ITokenStorage>(MockBehavior.Strict);
        tokenStorageMock
            .Setup(x => x.StoreAuthenticationTokenAsync(It.IsAny<AuthenticationToken>()))
            .Returns(ValueTask.CompletedTask)
            .Verifiable();

        var keychainMock = KeychainMock.CreateWithKey("{D996084E-7AE1-449F-8A01-FDDF2F4309F3}");

        var secretGeneratorMock = new Mock<ISecretGenerator>();
        secretGeneratorMock
            .Setup(x => x.GenerateSecret())
            .Returns(Guid.NewGuid().ToString());

        var controller = new AuthenticationController(
            loggerMock.Object,
            tokenStorageMock.Object,
            keychainMock.Object,
            secretGeneratorMock.Object
        );

        var wrapped = await controller.EmitTokenAsync(canManageVacancies, canManagePermissions);

        Assert.That(
            wrapped.Result,
            Is.TypeOf<OkObjectResult>()
        );

        var result = wrapped.Result as OkObjectResult;

        Assert.That(
            result?.Value,
            Is.TypeOf<string>().And.Not.Empty
        );

        tokenStorageMock.Verify();
    }

    [Test]
    [TestCase(true, true)]
    [TestCase(true, false)]
    [TestCase(false, true)]
    [TestCase(false, false)]
    public async Task AuthenticatePassingValidToken(
        bool canMangePermissions,
        bool canManageVacancies
    )
    {
        const ulong userId = 0xDEADBEEF;
        const string secret = "{E33D4E5A-5AF5-4600-91F4-E7D7C052A8CD}";

        var loggerMock = new Mock<ILogger<AuthenticationController>>();

        var tokenStorageMock = new Mock<ITokenStorage>(MockBehavior.Strict);
        tokenStorageMock
            .Setup(
                x => x.ConsumeAuthenticationTokenAsync(
                    It.Is<string>(s => s.Equals(secret))
                )
            )
            .ReturnsAsync(new AuthenticationToken()
            {
                Secret = secret,
                CanManagePermissions = canMangePermissions,
                CanManageVacancies = canManageVacancies
            })
            .Verifiable();

        var keychainMock = KeychainMock.CreateWithKey("{77DFA8F4-429C-48B4-A71F-7399C1B75431}");

        var secretGeneratorMock = new Mock<ISecretGenerator>();

        var controller = new AuthenticationController(
            loggerMock.Object,
            tokenStorageMock.Object,
            keychainMock.Object,
            secretGeneratorMock.Object
        );

        var wrapped = await controller.AuthenticateAsync(userId, secret);

        Assert.That(
            wrapped.Result,
            Is.TypeOf<OkObjectResult>()
        );

        var result = wrapped.Result as OkObjectResult;

        Assert.That(
            result!.Value,
            Is.TypeOf<Cookie>()
        );

        var token = result.Value as Cookie;

        Assert.That(
            token!.AccessToken,
            Is.Not.Empty
        );
        Assert.That(
            token.CanManagePermissions,
            Is.EqualTo(canMangePermissions)
        );
        Assert.That(
            token.CanManageVacancies,
            Is.EqualTo(canManageVacancies)
        );

        tokenStorageMock.Verify();
    }

    [Test]
    public async Task AuthenticatePassingInvalidToken()
    {

    }
}
