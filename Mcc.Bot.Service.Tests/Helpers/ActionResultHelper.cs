using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;

namespace Mcc.Bot.Service.Tests.Helpers
{
    public static class ActionResultHelper
    {
        public static TContent UnwrapContentAsOkObjectResult<TContent>(
            this ActionResult<TContent> self
        )
            where TContent : class
        {
            var result = self.Result as OkObjectResult;
            Assert.That(
                result,
                Is.Not.Null,
                $"{self.GetType()} is not a type of {typeof(OkObjectResult)}"
            );

            var content = result!.Value as TContent;
            Assert.That(
                content,
                Is.Not.Null,
                $"Content is not a type of {typeof(TContent)}"
            );

            return content!;
        }
    }
}