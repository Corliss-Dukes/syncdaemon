using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace syncdaemon
{
    public class Builder
    {
        public IHostBuilder builder(string[] args)
        {
            var builder = new HostBuilder()
    .ConfigureAppConfiguration((hostingContext, config) =>
    {
        config.AddEnvironmentVariables();

        if (args != null)
        {
            config.AddCommandLine(args);
        }
    })
    .ConfigureServices((hostContext, services) =>
    {
        services.AddOptions();
        services.Configure<DaemonConfig>(hostContext.Configuration.GetSection("Daemon"));

        services.AddSingleton<IHostedService, DaemonService>();
    })
    .ConfigureLogging((hostingContext, logging) =>
    {
        logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
        logging.AddConsole();
    });
            return builder;
        }
    }
}
