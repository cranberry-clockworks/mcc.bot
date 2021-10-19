using Microsoft.EntityFrameworkCore;
using Mcc.Bot.Service.Models;
using Microsoft.AspNetCore.Builder;
using System;

namespace Mcc.Bot.Service.Data
{
    public class ServiceContext : DbContext
    {
        public ServiceContext(DbContextOptions<ServiceContext> options) : base(options)
        {
        }

        public DbSet<Permission> Permissions => Set<Permission>();
        public DbSet<Vacancy> Vacancies => Set<Vacancy>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Permission>()
                .HasKey(p => p.UserId);

            modelBuilder.Entity<Vacancy>()
                .HasKey(v => v.Id);
        }
    }
}