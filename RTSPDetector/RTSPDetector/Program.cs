using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System;
using System.Collections.Generic;
using System.Text;

namespace RTSPDetector.WindowsService
{
    internal class Program
    {
        private static IServiceProvider _serviceProvider;

        public static void Main(string[] args)
        {
            RegisterServices();
            DisposeServices();
        }

        private static void RegisterServices()
        {
            IConfiguration config = new ConfigurationBuilder()
            .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("Environment")}.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables()
            .Build();

            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(config)
                .CreateLogger();

            Log.Information("Service registration is starting...");
            var services = new ServiceCollection();

            services.AddSingleton<IConfiguration>(config);
            services.AddSingleton<BackgroundApplication>();

            _serviceProvider = services.BuildServiceProvider(true);

            Log.Information("Service registration complete.");
        }

        private static void DisposeServices()
        {
            Log.Information("Disposing services...");

            if (_serviceProvider == null)
                return;

            if (_serviceProvider is IDisposable)
                ((IDisposable)_serviceProvider).Dispose();

            Log.Information("Services have been disposed.");
        }
    }
}