using System;
using System.ComponentModel.DataAnnotations;

namespace Mcc.Bot.Service.Models;

public class VacancyHeader
{
    public Guid Id { get; init; }
    public string Title { get; init; } = string.Empty;
} 

public class Vacancy : VacancyHeader
{
    public ulong OwnerUserId { get; init; }
    public string Description { get; init; } = string.Empty;
    public DateTime Created { get; init; }
}