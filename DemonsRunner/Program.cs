using Microsoft.Extensions.Hosting;
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
            .UseContentRoot(App.CurrentDirectory)
            .ConfigureServices(App.ConfigureServices)
            ;
    }
}
