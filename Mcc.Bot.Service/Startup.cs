using Mcc.Bot.Service.Security;
using Mcc.Bot.Service.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using System.IO;
using System.Reflection;
using System;

namespace Mcc.Bot.Service;

internal class Startup
{
    public IConfiguration Configuration { get; }

    private static string GetXmlCommentsFilePath()
    {
        var basePath = AppContext.BaseDirectory;
        var fileName = typeof(Startup).GetTypeInfo().Assembly.GetName().Name + ".xml";
        return Path.Combine(basePath, fileName);
    }

    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddOptions<AuthenticationOptions>()
            .Bind(Configuration.GetSection(AuthenticationOptions.AuthenticationSection));

        services.AddOptions<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme)
            .Configure<IKeychain>(
                (options, keychain) =>
                {
                    options.TokenValidationParameters = new()
                    {
                        ValidateIssuer = true,
                        ValidIssuer = JwtConfigurator.Issuer,

                        ValidateAudience = true,
                        ValidAudience = JwtConfigurator.Audience,

                        ValidateLifetime = false,

                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = keychain.SigningKey,
                    };
                }
            );

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer();

        services.AddAuthorization(
            options =>
            {
                options.AddPolicy(
                    Policices.CanManageVacanciesPolicy,
                    Policices.CanManageVacanciesPolicyBuilder
                );
                options.AddPolicy(
                    Policices.CanManagePermissionsPolicy,
                    Policices.CanManagePermissionsPolicyBuilder
                );
            }
        );

        services.AddDbContext<ServiceContext>(
            options => options.UseNpgsql(
                Configuration.GetConnectionString("Database"),
                o => o.UseQuerySplittingBehavior(QuerySplittingBehavior.SingleQuery)
            ),
            ServiceLifetime.Transient
        );

        services.AddSingleton<IKeychain, Keychain>();

        services.AddSingleton<ISecretGenerator, SecretGenerator>();
        services.AddTransient<IVacancyStorage, VacancyStorage>();
        services.AddTransient<ITokenStorage, TokenStorage>();

        services.AddControllers();

        services.AddSwaggerGen(
            c => {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Service", Version = "v1" });
                c.IncludeXmlComments(GetXmlCommentsFilePath());

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
                        Array.Empty<string>()
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
