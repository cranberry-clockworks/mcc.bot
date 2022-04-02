using Mcc.Bot.Service.Controllers;
using Mcc.Bot.Service.Data;
using Mcc.Bot.Service.Models;
using Mcc.Bot.Service.Tests.Extensions;
using Mcc.Bot.Service.Tests.Mocks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mcc.Bot.Service.Tests.Controllers;

[TestFixture]
internal class VacanciesControllerTests
{
    private static VacanciesController BuildController(Mock<IVacancyStorage> storageMock)
    {
        var loggerMock = new Mock<ILogger<VacanciesController>>(MockBehavior.Loose);
        var httpContextAccessorMock = new Mock<HttpContextAccessor>(MockBehavior.Loose);
        return new VacanciesController(
            loggerMock.Object,
            storageMock.Object,
            httpContextAccessorMock.Object
        );
    }

    private static VacanciesController BuildController(
        Mock<IVacancyStorage> storageMock,
        Mock<IHttpContextAccessor> httpContextAccessorMock
    )
    {
        var loggerMock = new Mock<ILogger<VacanciesController>>(MockBehavior.Loose);
        return new VacanciesController(
            loggerMock.Object,
            storageMock.Object,
            httpContextAccessorMock.Object
        );
    }

    [Test]
    public async Task GetAllOpenedVacanciesWhenEmpty()
    {
        var storageMock = new Mock<IVacancyStorage>(MockBehavior.Strict);
        storageMock
            .Setup(s => s.ListAllVacanciesHeadersAsync().Result)
            .Returns(new List<VacancyShortDescription>())
            .Verifiable();

        var controller = BuildController(storageMock);
        var response = await controller.GetAllVacanciesAsync();
        var content = response.UnwrapResponseAsOkObjectResult();

        Assert.That(content.Any(), Is.False);

        storageMock.Verify();
    }

    [Test]
    public async Task GetAllOpenedVacanciesWhenThereAreSome()
    {
        var stored = Enumerable
            .Range(0, 10)
            .Select(
                _ => new VacancyShortDescription
                {
                    Id = Guid.NewGuid(),
                    Title = Guid.NewGuid().ToString(),
                }
            )
            .ToList();
    
        var storageMock = new Mock<IVacancyStorage>(MockBehavior.Strict);
        storageMock
            .Setup(s => s.ListAllVacanciesHeadersAsync().Result)
            .Returns(stored)
            .Verifiable();

        var controller = BuildController(storageMock);
        var response = await controller.GetAllVacanciesAsync();
        var content = response.UnwrapResponseAsOkObjectResult();

        Assert.That(content.SequenceEqual(stored));

        storageMock.Verify();
    }

    [Test]
    public async Task GetVacancyDescryptionByWrongIdAsync()
    {
        var storageMock = new Mock<IVacancyStorage>(MockBehavior.Strict);
        storageMock
            .Setup(s => s.GetVacancyByIdAsync(It.IsAny<Guid>()).Result)
            .Throws<InvalidOperationException>();
        
        var controller = BuildController(storageMock);
        var response = await controller.GetVacancyDescriptionAsync(Guid.NewGuid());

        Assert.That(response.Result, Is.TypeOf<NotFoundResult>());

        storageMock.Verify();
    }

    [Test]
    public async Task GetVacancyDescriptionByRightIdAsync()
    {
        var stored = new Vacancy
        {
            Id = Guid.NewGuid(),
            Title = Guid.NewGuid().ToString(),
            OwnerUserId = 100,
            Description = Guid.NewGuid().ToString(),
            Created = DateTime.UtcNow,
        };

        var storageMock = new Mock<IVacancyStorage>(MockBehavior.Strict);
        storageMock
            .Setup(s => s.GetVacancyByIdAsync(It.IsAny<Guid>()).Result)
            .Returns(stored)
            .Verifiable();

        var controller = BuildController(storageMock);
        var response = await controller.GetVacancyDescriptionAsync(Guid.NewGuid());
        var received = response.UnwrapResponseAsOkObjectResult();

        Assert.That(received, Is.EqualTo(stored));

        storageMock.Verify();
    }

    [Test]
    [TestCase("")]
    [TestCase("Foo")]
    public async Task OpenVacancyWithBadParameters(string description)
    {
        var storageMock = new Mock<IVacancyStorage>(MockBehavior.Strict);
        var httpContextAccessorMock = HttpContextAccessorMock.CreateWithUserId(10);
        var controller = BuildController(storageMock, httpContextAccessorMock);
        
        var response = await controller.OpenVacancyAsync(string.Empty, description);
        Assert.That(response.Result, Is.TypeOf<BadRequestObjectResult>());

        storageMock.Verify();
    }

    [Test]
    public async Task OpenVacancy()
    {
        var ownerId = 10ul;
        var title = Guid.NewGuid().ToString();
        var description = Guid.NewGuid().ToString();
        
        var storageMock = new Mock<IVacancyStorage>(MockBehavior.Strict);
        Vacancy? created = null;
        storageMock
            .Setup(s => s.AddVacancyAsync(It.IsAny<Vacancy>()))
            .Callback<Vacancy>(v => created = v)
            .Returns(Task.CompletedTask);

        var httpContextAccessorMock = HttpContextAccessorMock.CreateWithUserId(ownerId);
        var controller = BuildController(storageMock, httpContextAccessorMock);

        var response = await controller.OpenVacancyAsync(title, description);
        var result = response.UnwrapResponseAsCreatedAtRouteResult();

        Assert.That(created, Is.Not.Null);

        Assert.That(result.Id, Is.EqualTo(created!.Id));
        Assert.That(result.OwnerUserId, Is.EqualTo(ownerId));
        Assert.That(result.Title, Is.EqualTo(title));
        Assert.That(result.Description, Is.EqualTo(description));
    }

    [Test]
    public async Task CloseVacancyByWrongId()
    {
        var storageMock = new Mock<IVacancyStorage>(MockBehavior.Strict);
        storageMock
            .Setup(s => s.RemoveVacancyByIdAsync(It.IsAny<Guid>()))
            .ThrowsAsync(new InvalidOperationException())
            .Verifiable();

        var httpContextAccessorMock = HttpContextAccessorMock.CreateWithUserId(0ul);
        var controller = BuildController(storageMock, httpContextAccessorMock);

        var id = Guid.NewGuid();

        var response = await controller.CloseVacancyAsync(id);
        
        var result = response.UnwrapResponseAsNotFoundObjectResult();
        Assert.That(result, Is.EqualTo(id));

        storageMock.Verify();
    }

    [Test]
    public async Task CloseVacancy()
    {
        var storageMock = new Mock<IVacancyStorage>(MockBehavior.Strict);
        storageMock
            .Setup(s => s.RemoveVacancyByIdAsync(It.IsAny<Guid>()))
            .Returns(Task.CompletedTask);

        var httpContextAccessorMock = HttpContextAccessorMock.CreateWithUserId(0ul);
        var controller = BuildController(storageMock, httpContextAccessorMock);

        var id = Guid.NewGuid();

        var response = await controller.CloseVacancyAsync(id);
        
        var result = response.UnwrapResponseAsOkObjectResult();
        Assert.That(result, Is.EqualTo(id));

        storageMock.Verify();
    }
}
