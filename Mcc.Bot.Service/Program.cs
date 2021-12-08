using Mcc.Bot.Service.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Mcc.Bot.Service;

/// <summary>
/// A main class of the service application.
/// </summary>
internal class Program
{
    public static void Main(string[] args)
    {
        CreateHostBuilder(args).Build().MigrateDatabase().Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
            });
}

internal static class IHostExtension
{
    public static IHost MigrateDatabase(this IHost host)
    {
        using var scope = host.Services.CreateScope();
        using var context = scope.ServiceProvider.GetRequiredService<ServiceContext>();
        context.Database.Migrate();
        return host;
    }
}
