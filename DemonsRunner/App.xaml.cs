using DemonsRunner.BuisnessLayer.Services;
using DemonsRunner.BuisnessLayer.Services.Interfaces;
using DemonsRunner.DAL.Repositories;
using DemonsRunner.DAL.Repositories.Interfaces;
using DemonsRunner.DAL.Storage;
using DemonsRunner.DAL.Storage.Interfaces;
using DemonsRunner.Domain.Models;
using DemonsRunner.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
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

        private static readonly string UniqueEventName = AppDomain.CurrentDomain.FriendlyName;

        #endregion

        #region --Properties--

        public static string CurrentDirectory => IsDesignMode ? Path.GetDirectoryName(GetSourceCodePath()) : Environment.CurrentDirectory;

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
            //.AddScoped<IRepository<PHPDemon>, FileRepository>()
            //.AddSingleton(new StorageFile("data.json"))
            //.AddScoped(provider => new StorageFile("data.json"))
            .AddTransient<IStorageFile, StorageFile>(p => new StorageFile("data.json"))
            .AddTransient<IFileRepository<PHPDemon>, FileRepository>()
            .AddTransient<IFileService, FileService>()
            .AddTransient<IFileDialogService, FileDialogService>()
            .AddTransient<IScriptConfigureService, ScriptConfigureService>()
            .AddTransient<IScriptExecutorService, ScriptExecutorService>()
            .AddTransient<IResponseFactory, ResponseFactory>()
            .AddSingleton<IDataBus, DataBusService>()
            .AddTransient<IScriptExecutorViewModelFactory, PHPScriptExecutorViewModelFactory>()
            .AddSingleton<MainWindowViewModel>()
            .AddSingleton<FilesPanelViewModel>()
            .AddSingleton<WorkSpaceViewModel>()
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

        private static string GetSourceCodePath([CallerFilePath] string path = null) => string.IsNullOrWhiteSpace(path) 
            ? throw new ArgumentNullException(nameof(path)) : path;

        #endregion
    }
}
