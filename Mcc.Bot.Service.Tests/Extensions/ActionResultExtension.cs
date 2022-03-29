using System;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;

namespace Mcc.Bot.Service.Tests.Extensions;

internal static class ActionResultExtension
{
    /// <summary>
    /// Unwraps <see cref="ActionResult"/> as <see cref="OkObjectResult"/> and retrives
    /// the inner response object.
    /// </summary>
    /// <param name="self">
    /// The unwrapped action result.
    /// </param>
    /// <typeparam name="TContent">
    /// The unwrapped type of the response object.
    /// </typeparam>
    /// <returns>
    /// The inner response object.
    /// </returns>
    /// <exception cref="InvalidCastException">
    /// Raised when failed to cast inner response object to the demanded type.
    /// </exception>
    /// <remarks>
    /// Asserts if the unwrapped <see cref="ActionResult"/> is not of type of
    /// <see cref="OkObjectResult"/>.
    /// </remarks>
    public static TContent UnwrapResponseAsOkObjectResult<TContent>(
        this ActionResult<TContent> self
    )
    {
        var result = self.Result as OkObjectResult;
        Assert.That(
            result,
            Is.Not.Null,
            $"{self.GetType()} is not a type of {typeof(OkObjectResult)}"
        );

        Assert.That(
            result!.Value,
            Is.Not.Null,
            "Result doesn't contain a value"
        );

        return (TContent)result.Value!;
    }

    /// <summary>
    /// Unwraps <see cref="ActionResult"/> as <see cref="CreatedAtRouteResult"/> and retrives
    /// the inner response object.
    /// </summary>
    /// <param name="self">
    /// The unwrapped action result.
    /// </param>
    /// <typeparam name="TContent">
    /// The unwrapped type of the response object.
    /// </typeparam>
    /// <returns>
    /// The inner response object.
    /// </returns>
    /// <exception cref="InvalidCastException">
    /// Raised when failed to cast inner response object to the demanded type.
    /// </exception>
    /// <remarks>
    /// Asserts if the unwrapped <see cref="ActionResult"/> is not of type of
    /// <see cref="CreatedAtRouteResult"/>.
    /// </remarks>
    public static TContent UnwrapResponseAsCreatedAtRouteResult<TContent>(
        this ActionResult<TContent> self
    )
    {
        var result = self.Result as CreatedAtRouteResult;
        Assert.That(
            result,
            Is.Not.Null,
            $"{self.GetType()} is not a type of {typeof(CreatedAtRouteResult)}"
        );

        Assert.That(
            result!.Value,
            Is.Not.Null,
            "Result doesn't contain a value"
        );

        return (TContent)result.Value!;
    }

    /// <summary>
    /// Unwraps <see cref="ActionResult"/> as <see cref="NotFoundObjectResult"/> and retrives
    /// the inner response object.
    /// </summary>
    /// <param name="self">
    /// The unwrapped action result.
    /// </param>
    /// <typeparam name="TContent">
    /// The unwrapped type of the response object.
    /// </typeparam>
    /// <returns>
    /// The inner response object.
    /// </returns>
    /// <exception cref="InvalidCastException">
    /// Raised when failed to cast inner response object to the demanded type.
    /// </exception>
    /// <remarks>
    /// Asserts if the unwrapped <see cref="ActionResult"/> is not of type of
    /// <see cref="NotFoundObjectResult"/>.
    /// </remarks>
    public static TContent UnwrapResponseAsNotFoundObjectResult<TContent>(
        this ActionResult<TContent> self
    )
    {
        var result = self.Result as NotFoundObjectResult;
        Assert.That(
            result,
            Is.Not.Null,
            $"{self.GetType()} is not a type of {typeof(NotFoundObjectResult)}"
        );

        Assert.That(
            result!.Value,
            Is.Not.Null,
            "Result doesn't contain a value"
        );

        return (TContent)result.Value!;
    }
}