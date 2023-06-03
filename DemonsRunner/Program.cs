using DemonsRunner.DAL.Storage;
using Microsoft.Extensions.Hosting;
using Serilog;
using System;
using System.IO;
using System.Windows;
using System.Xml.Linq;

namespace DemonsRunner
{
    internal class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            MessageBox.Show("IN MAIN");
            try
            {
                App app = new();
                app.Run();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
           
        }

        public static IHostBuilder CreateHostBuilder(string[] args) => Host
            .CreateDefaultBuilder(args)
            .UseSerilog((host, loggingConfiguration) =>
            {
                string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                string appName = AppDomain.CurrentDomain.FriendlyName;

                string logFileDirectoryPath = Path.Combine(appDataPath, appName);
                if (!Directory.Exists(logFileDirectoryPath))
                {
                    Directory.CreateDirectory(logFileDirectoryPath);
                }

                string logFileName = "log.txt";
                string logFileFullPath = Path.Combine(logFileDirectoryPath, logFileName);

                loggingConfiguration.MinimumLevel.Information();
#if DEBUG
                loggingConfiguration.WriteTo.Debug();
#else
                loggingConfiguration.WriteTo.File(logFileFullPath, rollingInterval: RollingInterval.Day);
#endif
            })
            .UseContentRoot(App.CurrentDirectory)
            .ConfigureServices(App.ConfigureServices)
            ;
    }
}
