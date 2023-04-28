using DemonsRunner.BuisnessLayer.Services;
using DemonsRunner.DAL.Repositories;
using DemonsRunner.Domain.Interfaces;
using DemonsRunner.Domain.Models;
using DemonsRunner.Domain.Repositories;
using DemonsRunner.Domain.Services;
using DemonsRunner.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Windows;

namespace DemonsRunner
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        #region --Fields--

        private static IHost? _host;

        private static readonly string UniqueEventName = "DemonsRunner";

        #endregion

        #region --Properties--

        public static bool IsDesignMode { get; private set; } = true;

        public static IServiceProvider Services => Host.Services;

        public static IHost Host => _host ??= Program.CreateHostBuilder(Environment.GetCommandLineArgs()).Build();

        #endregion

        #region --Constructors--



        #endregion

        #region --Methods--

        protected override async void OnStartup(StartupEventArgs e)
        {
            if (IsNewInstance())
            {
                EventWaitHandle eventWaitHandle = new(false, EventResetMode.AutoReset, UniqueEventName);
                Current.Exit += (sender, args) => eventWaitHandle.Close();
                var host = Host;
                base.OnStartup(e);
                await host.StartAsync();
                IsDesignMode = false;

                Services.GetRequiredService<MainWindow>().Show();
            }
        }

        protected override async void OnExit(ExitEventArgs e)
        {
            using var host = Host;
            base.OnExit(e);
            await host.StopAsync();
            Current.Shutdown();
        }

        internal static void ConfigureServices(HostBuilderContext host, IServiceCollection services) => services
            .AddSingleton<IRepository<PHPDemon>, FileRepository>()
            .AddSingleton<IFileService, FileService>()
            .AddSingleton<IFileDialogService, FileDialogService>()
            .AddSingleton<IScriptConfigureService, ScriptConfigureService>()
            .AddSingleton<IScriptExecutorService, ScripExecutorService>()
            .AddSingleton<MainWindowViewModel>()
            .AddTransient(s =>
            {
                var viewModel = s.GetRequiredService<MainWindowViewModel>();
                var window = new MainWindow { DataContext = viewModel };

                return window;
            })
            ;

        private bool IsNewInstance()
        {
            try
            {
                EventWaitHandle eventWaitHandle = EventWaitHandle.OpenExisting(UniqueEventName); // here will be exception if app is not even starting
                eventWaitHandle.Set();
                Shutdown();
            }
            catch (WaitHandleCannotBeOpenedException)
            {
                return true;
            }
            return false;
        }

        #endregion
    }
}
