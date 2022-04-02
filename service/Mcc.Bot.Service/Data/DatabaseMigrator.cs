using System.Linq;
using Mcc.Bot.Service.Models;
using Mcc.Bot.Service.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Mcc.Bot.Service.Data;

/// <summary>
/// The data base migration applier. 
/// </summary>
internal class DatabaseMigrator
{
    private readonly ILogger<DatabaseMigrator> logger;
    private readonly ServiceContext context;
    private readonly AuthenticationOptions authenticationOptions;

    /// <summary>
    /// Creates the migrator.
    /// </summary>
    /// <param name="logger">
    /// A logger to write messages.
    /// </param>
    /// <param name="context">
    /// The database context instance to perform migration for.
    /// </param>
    /// <param name="authenticationOptions">
    /// An authentication options that should be populated into the database.
    /// </param>
    public DatabaseMigrator(
        ILogger<DatabaseMigrator> logger,
        ServiceContext context,
        IOptions<AuthenticationOptions> authenticationOptions
    )
    {
        this.logger = logger;
        this.context = context;
        this.authenticationOptions = authenticationOptions.Value;
    }

    /// <summary>
    /// Perform migration and seed the data if needed.
    /// </summary>
    public void Migrate()
    {
        logger.LogInformation("Start database migration");
        
        context.Database.Migrate();
        Seed();
        
        logger.LogInformation("Finished database migration");
    }

    private void Seed()
    {
        logger.LogInformation("Start seeding values into database");
        
        SeedFirstAuthenticationToken();

        logger.LogInformation("Finished seeding values into database");
    }

    private void SeedFirstAuthenticationToken()
    {
        if (context.AuthenticationTokens.Any())
            return;
        
        if (authenticationOptions.FirstSecret.Length == 0)
            return;
        
        context.AuthenticationTokens.Add(
            new AuthenticationToken
            {
                Secret = authenticationOptions.FirstSecret,
                CanManagePermissions = true,
                CanManageVacancies = true
            }
        );
        
        context.SaveChanges();
    }
}