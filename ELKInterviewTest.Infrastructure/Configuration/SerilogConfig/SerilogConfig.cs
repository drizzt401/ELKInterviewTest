using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Core;
using Serilog.Exceptions;
using Serilog.Sinks.Elasticsearch;
using System;
using System.Reflection;

namespace ELKInterviewTest.Infrastructure
{
    public static class SerilogConfig
    {
        public static Logger GetLogger()
        {
            var env = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT");

            if (env is null) env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            // Get the configuration 
            var configuration = new ConfigurationBuilder()
                    .AddJsonFile("appsettings.Development.json", optional: false, reloadOnChange: true)
                    .Build();

            return new LoggerConfiguration()
                            .MinimumLevel.Verbose()
                            .Enrich.FromLogContext()
                            .Enrich.WithExceptionDetails()
                            .WriteTo.Debug()
                            .WriteTo.Console()
                            .WriteTo.Elasticsearch(ConfigureELS(configuration, env))
                            .CreateLogger();

        }
        private static ElasticsearchSinkOptions ConfigureELS(IConfigurationRoot configuration, string env)
        {
            return new ElasticsearchSinkOptions(new Uri(configuration["ELKService:Uri"]))
            {
                AutoRegisterTemplate = true,
                IndexFormat = $"{Assembly.GetExecutingAssembly().GetName().Name.ToLower()}-{env.ToLower().Replace(".", "-")}-{DateTime.UtcNow:yyyy-MM}",
                ModifyConnectionSettings = x => x.BasicAuthentication("elastic", configuration["ELKService:password"])
            };
        }
    }
}
