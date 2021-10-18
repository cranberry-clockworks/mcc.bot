using Microsoft.EntityFrameworkCore;
using Mcc.Bot.Service.Models;

namespace Mcc.Bot.Service.Data
{
    public class ServiceContext : DbContext
    {
        public ServiceContext(DbContextOptions<ServiceContext> options) : base(options)
        {
            Migrate();
        }

        public DbSet<Permission> Permissions { get; set; }
        public DbSet<Vacancy> Vacancies { get; set; }

        private object _guard = new();
        private static bool _migrated;

        private void Migrate()
        {
            lock (_guard)
            {
                if (_migrated)
                    return;

                Database.Migrate();
                _migrated = true;
            }
        }
    }
}