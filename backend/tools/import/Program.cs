﻿using System;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Pims.Tools.Core.Exceptions;
using Pims.Tools.Core.Keycloak;
using Pims.Tools.Core.Keycloak.Configuration;
using Pims.Tools.Core.OpenIdConnect;
using Pims.Tools.Core.OpenIdConnect.Configuration;

namespace Pims.Tools.Import
{
    /// <summary>
    /// Program class, provides a console application that can read and iterate through a JSON file and make HTTP requests to an endpoint.
    /// </summary>
    /// <reference url="https://docs.microsoft.com/en-us/aspnet/core/fundamentals/logging/?view=aspnetcore-3.1"/>
    static class Program
    {

        #region Methods
        /// <summary>
        /// Entry point to program.
        /// </summary>
        /// <param name="args"></param>
        static async Task<int> Main(string[] args)
        {
            var config = Configure(args)
                .Build();

            var services = new ServiceCollection()
                .Configure<ToolOptions>(config)
                .Configure<KeycloakRequestOptions>(config)
                .Configure<KeycloakOptions>(config.GetSection("Keycloak"))
                .Configure<OpenIdConnectOptions>(config.GetSection("Keycloak"))
                .Configure<ImportOptions>(config.GetSection("Import"))
                .AddSingleton<IConfiguration>(config)
                .AddLogging(options =>
                {
                    options.AddConfiguration(config.GetSection("Logging"));
                    options.AddConsole();
                })
                .Configure<LoggerFilterOptions>(options =>
                {
                    options.MinLevel = LogLevel.Information;
                })
                .AddTransient<Startup>()
                .AddTransient<JwtSecurityTokenHandler>()
                .AddScoped<IKeycloakRequestClient, KeycloakRequestClient>()
                .AddTransient<IImporter, Importer>()
                .AddSingleton<IOpenIdConnector, OpenIdConnector>();

            services.AddHttpClient("Pims.Tools.Import", client => { });

            var provider = services.BuildServiceProvider();

            var logger = provider.GetService<ILogger<Startup>>();

            var result = 0;
            try
            {
                result = await provider.GetService<Startup>().Run(args);
            }
            catch (HttpResponseException ex)
            {
                logger.LogCritical(ex, $"An HTTP request failed - {ex.StatusCode}: {ex.Details}");
                result = 1;
            }
            catch (Exception ex)
            {
                logger.LogCritical(ex, "An unhandled error has occurred.");
                result = 1;
            }

            provider.Dispose();
            return result;
        }

        /// <summary>
        /// Configure host.
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        private static IConfigurationBuilder Configure(string[] args)
        {
            DotNetEnv.Env.Load();
            string environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            return new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .AddJsonFile($"appsettings.{environment}.json", optional: true)
                .AddEnvironmentVariables()
                .AddCommandLine(args ?? new string[0]);
        }
        #endregion
    }
}
