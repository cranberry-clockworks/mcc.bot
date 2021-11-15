using System;

namespace Mcc.Bot.Service.Models;

public class Vacancy
{
    public Guid Id { get; init; }
    public ulong OwnerUserId { get; init; }
    public string Title { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public DateTime Created { get; init; }
}

public class VacancyHeader
{
    public Guid Id { get; init; }
    public string Title { get; init; } = string.Empty;
}