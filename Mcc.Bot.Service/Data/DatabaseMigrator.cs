using System.Linq;
using Mcc.Bot.Service.Models;
using Mcc.Bot.Service.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Mcc.Bot.Service.Data;

internal class DatabaseMigrator
{
    private readonly ILogger<DatabaseMigrator> logger;
    private readonly ServiceContext context;
    private readonly AuthenticationOptions authenticationOptions;

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