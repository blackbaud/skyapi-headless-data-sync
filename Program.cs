using Blackbaud.HeadlessDataSync.Services;
using Blackbaud.HeadlessDataSync.Services.DataSync;
using Blackbaud.HeadlessDataSync.Services.SkyApi;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Configuration;
using System.IO;

namespace Blackbaud.HeadlessDataSync
{
    class Program
    {
        public static IConfigurationRoot Configuration;
        private const string ENV_VAR_PREFIX = "BBHeadlessDataSync_";

        static void Main(string[] args)
        {
            Configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables(ENV_VAR_PREFIX)
                .Build();

            // set up DI
            var services = new ServiceCollection()
                .AddLogging(builder => {
                    builder.AddConfiguration(Configuration.GetSection("Logging"));
                    builder.AddConsole();
                 })
                .AddOptions()
                .Configure<AppSettings>(options => Configuration.GetSection("AppSettings").Bind(options))
                .AddSingleton<IAuthenticationService, AuthenticationService>()
                .AddSingleton<IConstituentsService, ConstituentsService>()
                .AddSingleton<IDataStorageService, DataStorageService>()
                .AddSingleton<IDataSyncService, DataSyncService>()
                .AddTransient<SyncApp, SyncApp>();

            // Add data protection
            services.AddDataProtection();

            var serviceProvider = services.BuildServiceProvider();

            // configure console logging
            serviceProvider.GetService<ILoggerFactory>()
                .CreateLogger<Program>();

            // run data sync
            serviceProvider.GetService<SyncApp>().Run(args);
        }
    }
}
