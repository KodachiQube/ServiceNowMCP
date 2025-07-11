using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using ServiceNow.Core.Interfaces;
using ServiceNow.Services.Services;
using ServiceNow.Services.Clients;
using ServiceNow.Services.Configuration;
using ServiceNow.Functions.MCP;
using Polly;
using Polly.Extensions.Http;
using Microsoft.Extensions.Http;
using System.Net.Http;
using System;
using System.Threading.Tasks;

namespace ServiceNow.Functions
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            // Check if running as MCP server
            if (args.Length > 0 && args[0] == "--mcp-server")
            {
                await RunMcpServer();
                return;
            }

            // Otherwise run as Azure Function
            var host = new HostBuilder()
                .ConfigureFunctionsWebApplication()
                .ConfigureAppConfiguration((context, config) =>
                {
                    config.AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                          .AddEnvironmentVariables();
                })
                .ConfigureServices((context, services) =>
                {
                    services.AddApplicationInsightsTelemetryWorkerService();
                    services.ConfigureFunctionsApplicationInsights();
                    
                    // Configure ServiceNow settings
                    services.Configure<ServiceNowConfiguration>(context.Configuration.GetSection("ServiceNow"));
                    
                    // Add HttpClient with retry policy
                    services.AddHttpClient<ServiceNowClient>()
                        .AddPolicyHandler(GetRetryPolicy());
                    
                    // Register services
                    services.AddScoped<IIncidentService, IncidentService>();
                    services.AddScoped<IChangeRequestService, ChangeRequestService>();
                    services.AddScoped<IUserService, UserService>();
                    services.AddScoped<ITableService, TableService>();
                    
                    // Register MCP server
                    services.AddSingleton<ServiceNowMcpServer>();
                })
                .Build();

            host.Run();
        }

        private static async Task RunMcpServer()
        {
            var services = new ServiceCollection();
            
            // Add configuration
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("local.settings.json", optional: true)
                .AddEnvironmentVariables()
                .Build();
            
            services.AddSingleton<IConfiguration>(configuration);
            services.Configure<ServiceNowConfiguration>(configuration.GetSection("ServiceNow"));
            
            // Add logging
            services.AddLogging();
            
            // Add HttpClient
            services.AddHttpClient<ServiceNowClient>()
                .AddPolicyHandler(GetRetryPolicy());
            
            // Register services
            services.AddScoped<IIncidentService, IncidentService>();
            services.AddScoped<IChangeRequestService, ChangeRequestService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<ITableService, TableService>();
            
            // Register MCP server
            services.AddSingleton<ServiceNowMcpServer>();
            
            var serviceProvider = services.BuildServiceProvider();
            var mcpServer = serviceProvider.GetRequiredService<ServiceNowMcpServer>();
            
            // Start MCP server on stdio
            await mcpServer.StartAsync(Console.OpenStandardInput(), Console.OpenStandardOutput());
        }
        
        private static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
        {
            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .OrResult(msg => !msg.IsSuccessStatusCode)
                .WaitAndRetryAsync(
                    3,
                    retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                    onRetry: (outcome, timespan, retryCount, context) =>
                    {
                        Console.WriteLine($"Retry {retryCount} after {timespan} seconds");
                    });
        }
    }
}