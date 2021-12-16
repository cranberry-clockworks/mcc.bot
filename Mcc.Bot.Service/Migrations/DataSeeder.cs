using Mcc.Bot.Service.Security;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Configuration;

namespace Mcc.Bot.Service.Migrations;

internal class DataSeeder
{
    private readonly IConfiguration configuration;

    public DataSeeder()
    {
        configuration = new ConfigurationBuilder()
            .AddEnvironmentVariables()
            .Build();
    }


    public void SeedTo(MigrationBuilder builder)
    {
        SeedFirstSecret(builder);
    }

    private void SeedFirstSecret(MigrationBuilder builder)
    {
        var options = configuration.GetAuthenticationOptions();
        if (string.IsNullOrEmpty(options.FirstSecret))
            return;

        builder.InsertData(
            table: "AuthenticationTokens",
            columns: new[] { "Secret", "CanManagePermissions", "CanManageVacancies" },
            values: new object[,] {
                { options.FirstSecret, true, true },
            }
        );
    }
}
