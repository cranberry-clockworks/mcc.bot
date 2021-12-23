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
    public async Task EmitTokenTest(bool canManageVacancies, bool canManagePermissions)
    {
        var loggerMock = new Mock<ILogger<AuthenticationController>>();

        var tokenStorageMock = new Mock<ITokenStorage>(MockBehavior.Strict);
        tokenStorageMock
            .Setup(x => x.StoreAuthenticationToken(It.IsAny<AuthenticationToken>()))
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
}
