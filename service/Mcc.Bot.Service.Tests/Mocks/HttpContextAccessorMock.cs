using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Moq;

namespace Mcc.Bot.Service.Tests.Mocks;

/// <summary>
/// Provides methods to create mocks of <see cref="IHttpContextAccessor"/>.
/// </summary>
internal static class HttpContextAccessorMock
{
    /// <summary>
    /// Creates mock with the context that contains a user identity with given id.
    /// </summary>
    /// <param name="id">
    /// The id of the user.
    /// </param>
    /// <returns>
    /// The mock of <see cref="IHttpContextAccessor"/> with user identity.
    /// </returns>
    public static Mock<IHttpContextAccessor> CreateWithUserId(ulong id)
    {
        var context = new DefaultHttpContext()
        {
            User = new ClaimsPrincipal(
                new ClaimsIdentity(
                    new[]
                    {
                        new Claim(ClaimTypes.Name, id.ToString())
                    }
                )
            )
        };

        var mock = new Mock<IHttpContextAccessor>(MockBehavior.Strict);
        mock.Setup(i => i.HttpContext).Returns(context);

        return mock;
    }
}