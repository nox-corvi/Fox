using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Nox;

namespace Fox;

internal class Program
{
    //public static string CNStr = "Server=localhost;Encrypt=False;TrustServerCertificate=False;Database=qo;User Id=qu;Password=yS~QmDAGW:-7nCsa";

    /// <summary>
    ///  The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main()
    {
        var builder = Host.CreateDefaultBuilder()
            .ConfigureAppConfiguration((context, config) =>
            {
                config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
                config.AddEnvironmentVariables();
            })
            .ConfigureLogging((context, logging) =>
            {
                logging.ClearProviders();
                logging.AddProvider(new XLogProvider(context.Configuration));
            })
            .ConfigureServices((context, services) =>
            {
                //services.AddSingleton<XAuthPool.XAuth>();
                //services.AddSingleton<XLogPool.XLog>();
                //services.AddSingleton<QoPool.Qo>();
                //services.AddSingleton<TodoPool.Todo>();
                //services.AddSingleton<ACGDataPool.ACG>();

                services.AddSingleton(provider =>
                {
                    var Korekuta = new Nox.Korekuta();

                    //Korekuta.AddData(provider.GetRequiredService<XAuthPool.XAuth>());
                    //Korekuta.AddData(provider.GetRequiredService<XLogPool.XLog>());
                    //Korekuta.AddData(provider.GetRequiredService<TodoPool.Todo>());
                    //Korekuta.AddData(provider.GetRequiredService<QoPool.Qo>());
                    //Korekuta.AddData(provider.GetRequiredService<ACGDataPool.ACG>());

                    return Korekuta;
                });

                //services.AddSingleton<Login>();
                services.AddSingleton<MainForm>();
            });

        using IHost host = builder.Build();

        var Korekuta = host.Services.GetRequiredService<Nox.Korekuta>();
        Korekuta.Host = host;

        Korekuta.SetLocaleService(new Nox.LocaleService());

        var logger = host.Services.GetRequiredService<ILogger<Program>>();
        logger.LogInformation("app configured");

        //var XAuth = host.Services.GetRequiredService<XAuthPool.XAuth>();
        //using var XAuthTimer = XAuthPool.XAuth.GetTokenRenewTimer(XAuth,
        //    Global.GetConfigValue("XAuth:Token"), Global.GetConfigValue("XAuth:Secret"));

        //using var QoTimer = XAuthPool.XAuth.GetTokenRenewTimer(XAuth,
        //    Global.GetConfigValue("Qo:Token"), Global.GetConfigValue("Qo:Secret"));

        //using var TodoTimer = XAuthPool.XAuth.GetTokenRenewTimer(XAuth,
        //    Global.GetConfigValue("Todo:Token"), Global.GetConfigValue("Todo:Secret"));

        //using var AcgTimer = XAuthPool.XAuth.GetTokenRenewTimer(XAuth,
        //    Global.GetConfigValue("ACG:Token"), Global.GetConfigValue("ACG:Secret"));

        //ApplicationConfiguration.Initialize();

        var mainForm = host.Services.GetRequiredService<MainForm>();
        Application.Run(mainForm);
    }
}