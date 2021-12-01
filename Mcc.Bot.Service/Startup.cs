using Mcc.Bot.Service.Authentication;
using Mcc.Bot.Service.Authorization;
using Mcc.Bot.Service.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

namespace Mcc.Bot.Service;

public class Startup
{
    public IConfiguration Configuration { get; }

    private string databaseConnectionString;
    private string signingKey;

    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;

        databaseConnectionString = Configuration.GetConnectionString("Database");
        signingKey = Configuration.GetAuthenticationSigningKey();
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(
            o =>
            {
                o.TokenValidationParameters = new()
                {
                    ValidateIssuer = true,
                    ValidIssuer = AuthenticationOptions.TokenIssuer,

                    ValidateAudience = true,
                    ValidAudience = AuthenticationOptions.TokenAudience,

                    ValidateLifetime = false,

                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = AuthenticationOptions.BuildSigningKey(signingKey)
                };
            }
        );
        services.AddAuthorization(
            options => options.AddPolicy(
                Policices.CanManageVacanciesPolicy,
                Policices.CanManageVacanciesPolicyBuilder
            )
        );
        services.AddDbContext<ServiceContext>(
            options => options.UseNpgsql(
                databaseConnectionString,
                o => o.UseQuerySplittingBehavior(QuerySplittingBehavior.SingleQuery)
            ),
            ServiceLifetime.Transient
        );

        services.AddTransient<IVacancyStorage, VacancyStorage>();
        services.AddTransient<IPermissionStorage, PermissionStorage>();

        services.AddControllers();

        services.AddSwaggerGen(
            c => {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Service", Version = "v1" });

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.Http,
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Scheme = "Bearer",
                    Description = "Please insert JWT token into field."
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        System.Array.Empty<string>()
                    }
                });
            }
        );
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
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
    }
}
