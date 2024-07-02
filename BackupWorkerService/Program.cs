//using BackupWorkerService;

//IHost host = Host.CreateDefaultBuilder(args)
//    .ConfigureServices(services =>
//    {
//        services.AddHostedService<Worker>();
//    })
//    .Build();

//await host.RunAsync();


using BackupWorkerService;

//namespace BackupWorkerService
//{
//    public class Program
//    {
//        public static void Main(string[] args)
//        {
//            CreateHostBuilder(args).Build().Run();
//        }

//        public static IHostBuilder CreateHostBuilder(string[] args) =>
//            Host.CreateDefaultBuilder(args)
//                .UseWindowsService()
//                .ConfigureAppConfiguration((hostingContext, config) =>
//                {
//                    var configFileName = "CompanySettings.json";
//                    if (File.Exists(configFileName))
//                    {
//                        config.AddJsonFile(configFileName, optional: false, reloadOnChange: true);
//                    }
//                    else
//                    {
//                        throw new InvalidOperationException("Configuration file not found.");
//                    }
//                })
//                .ConfigureServices((hostContext, services) =>
//                {
//                    services.AddHttpClient();
//                    services.AddHostedService<Worker>();
//                });
//    }
//}

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using static BackupWorkerService.Worker;

public class Program
{
    public static void Main(string[] args)
    {
        CreateHostBuilder(args).Build().Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .UseWindowsService()
            .ConfigureServices((hostContext, services) =>
            {
                services.AddOptions<WorkerOptions>()
                        .Bind(hostContext.Configuration.GetSection("WorkerOptions"));
                services.AddHttpClient();
                services.AddHostedService<Worker>();
            });
}


//using BackupWorkerService;
//using Microsoft.Extensions.Logging.Configuration;
//using Microsoft.Extensions.Logging.EventLog;

//HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);
//builder.Services.AddWindowsService(options =>
//{
//    options.ServiceName = "BackupWorkerService";
//});

//LoggerProviderOptions.RegisterProviderOptions<
//    EventLogSettings, EventLogLoggerProvider>(builder.Services);

//builder.Services.AddHttpClient();
//builder.Services.AddHostedService<Worker>();

//IHost host = builder.Build();
//host.Run();