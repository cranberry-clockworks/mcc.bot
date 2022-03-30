using System;
using System.ComponentModel.DataAnnotations;

namespace Mcc.Bot.Service.Models;

/// <summary>
/// A short description of the vacancy
/// </summary>
public record VacancyShortDescription
{
    /// <summary>
    /// A unique id of the vacancy.
    /// </summary>
    [Key]
    public Guid Id { get; init; }

    /// <summary>
    /// A title of the vacancy.
    /// </summary>
    public string Title { get; init; } = string.Empty;
}

/// <summary>
/// A full description of the vacancy.
/// </summary>
public record Vacancy : VacancyShortDescription
{
    /// <summary>
    /// An id of the user that created and owns the vacancy.
    /// </summary>
    public ulong OwnerUserId { get; init; }

    /// <summary>
    /// A full description of the vacancy.
    /// </summary>
    public string Description { get; init; } = string.Empty;

    /// <summary>
    /// A time when vacancy is created. Stored in the UTC format.
    /// </summary>
    public DateTime Created { get; init; }
}