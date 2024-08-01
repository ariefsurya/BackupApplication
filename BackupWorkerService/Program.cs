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
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;
using static BackupWorkerService.Worker;

public class Program
{
    public static void Main(string[] args)
    {
        Directory.CreateDirectory("C:\\logs");
        Serilog.Debugging.SelfLog.Enable(msg => File.AppendAllText("C:\\logs\\serilog-selflog.txt", msg + Environment.NewLine));

        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console(theme: AnsiConsoleTheme.Code)
            .WriteTo.File("C:\\logs\\myapp.txt", rollingInterval: RollingInterval.Day)
            .CreateLogger();

        try
        {
            Log.Information("Starting up");
            CreateHostBuilder(args).Build().Run();
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Application start-up failed");
        }
        finally
        {
            Log.Information("Shutting down");
            Log.CloseAndFlush();
        }
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .UseWindowsService()
            .ConfigureLogging(logging =>
            {
                logging.ClearProviders();
                logging.AddConsole();
                logging.SetMinimumLevel(LogLevel.Information);
            })
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