using DemonsRunner.DAL.Storage;
using Microsoft.Extensions.Hosting;
using Serilog;
using System;

namespace DemonsRunner
{
    internal class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            App app = new();
            app.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) => Host
            .CreateDefaultBuilder(args)
            .UseSerilog((host, loggingConfiguration) =>
            {
                var logStorageFile = new StorageFile("log.txt");
                loggingConfiguration.MinimumLevel.Information();
#if DEBUG
                loggingConfiguration.WriteTo.Debug();
#else
                loggingConfiguration.WriteTo.File(logStorageFile.FullPath, rollingInterval: RollingInterval.Day);
#endif
            })
            .UseContentRoot(App.CurrentDirectory)
            .ConfigureServices(App.ConfigureServices)
            ;
    }
}
