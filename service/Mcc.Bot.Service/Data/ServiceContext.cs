using Mcc.Bot.Service.Models;
using Microsoft.EntityFrameworkCore;

namespace Mcc.Bot.Service.Data;

/// <summary>
/// A database context provider for the service.
/// </summary>
internal class ServiceContext : DbContext
{
    /// <summary>
    /// Creates a database context.
    /// </summary>
    public ServiceContext(DbContextOptions<ServiceContext> options) : base(options)
    {
    }

    /// <summary>
    /// Storage of the tokens to authenticate users.
    /// </summary>
    public DbSet<AuthenticationToken> AuthenticationTokens => Set<AuthenticationToken>();

    /// <summary>
    /// Storage of the opened vacancies.
    /// </summary>
    public DbSet<Vacancy> Vacancies => Set<Vacancy>();
}