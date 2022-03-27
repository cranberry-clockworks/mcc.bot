using Mcc.Bot.Service.Controllers;
using Mcc.Bot.Service.Data;
using Mcc.Bot.Service.Models;
using Mcc.Bot.Service.Tests.Helpers;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mcc.Bot.Service.Tests.Controllers
{
    [TestFixture]
    public class VacanciesControllerTests
    {
        private static VacanciesController BuildController(Mock<IVacancyStorage> storageMock)
        {
            var loggerMock = new Mock<ILogger<VacanciesController>>(MockBehavior.Loose);
            return new VacanciesController(loggerMock.Object, storageMock.Object);
        }

        [Test]
        public async Task GetAllOpenedVacanciesWhenEmpty()
        {
            var storageMock = new Mock<IVacancyStorage>(MockBehavior.Strict);
            storageMock.Setup(s => s.ListAllVacanciesHeaders().Result)
                .Returns(new List<VacancyShortDescription>())
                .Verifiable();

            var controller = BuildController(storageMock);
            var response = await controller.GetAllVacanciesAsync();
            var content = response.UnwrapContentAsOkObjectResult();

            Assert.That(
                content.Any(),
                Is.False
            );

            storageMock.Verify();
        }
    }
}