using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Mcc.Bot.Service.Data;

namespace Mcc.Bot.Service
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        private string DatabaseConnectionString;

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;

            DatabaseConnectionString = Configuration.GetConnectionString("Database");
        }

        public void ConfigureServices(IServiceCollection services)
        {

            services.AddDbContext<ServiceContext>(
                o => o.UseNpgsql(
                    DatabaseConnectionString,
                    o => o.UseQuerySplittingBehavior(QuerySplittingBehavior.SingleQuery)
                ),
                ServiceLifetime.Transient
            );
            services.AddTransient<IVacancyStorage, VacancyStorage>();
            services.AddTransient<IPermissionStorage, PermissionStorage>();
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Service", Version = "v1" });
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(
                    c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "MCC Bot Service v1")
                );
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
