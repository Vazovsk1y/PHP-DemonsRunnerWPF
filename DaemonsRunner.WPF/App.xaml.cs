﻿using DaemonsRunner.BuisnessLayer.Extensions;
using DaemonsRunner.DAL.Extensions;
using DaemonsRunner.Infrastructure.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows;

namespace DaemonsRunner
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        #region --Fields--

        private static IHost? _host;

        public static readonly string Name = AppDomain.CurrentDomain.FriendlyName;

        #endregion

        #region --Properties--

        public static string CurrentDirectory => IsDesignMode ? Path.GetDirectoryName(GetSourceCodePath())! : Environment.CurrentDirectory;

        public static bool IsDesignMode { get; private set; } = true;

        public static IServiceProvider Services => Host.Services;

        public static IHost Host => _host ??= Program.CreateHostBuilder(Environment.GetCommandLineArgs()).Build();

        #endregion

        #region --Constructors--

        public App()
        {
            SetupGlobalExceptionsHandlers();
        }

        #endregion

        #region --Methods--

        public static void ConfigureServices(HostBuilderContext host, IServiceCollection services) => services
            .AddBuisnessLayer()
            .AddDataAccessLayer()
            .AddClientLayer()
            ;

        public bool IsNewAppProcessInstance()
        {
            try
            {
                EventWaitHandle eventWaitHandle = EventWaitHandle.OpenExisting(Name); // here will be exception if app is not even starting
                eventWaitHandle.Set();
            }
            catch (WaitHandleCannotBeOpenedException)
            {
                return true;
            }
            return false;
        }

        private static string GetSourceCodePath([CallerFilePath] string path = null) => string.IsNullOrWhiteSpace(path)
            ? throw new ArgumentNullException(nameof(path)) : path;

        private void SetupGlobalExceptionsHandlers()
        {
            DispatcherUnhandledException += (sender, e) =>
            {
                Log.Error(e.Exception, "Something went wrong in {nameofDispatcherUnhandledException}",
                    nameof(DispatcherUnhandledException));
                e.Handled = true;
                Current?.Shutdown();
            };

            AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
            {
                Log.Error(e.ExceptionObject as Exception, "Something went wrong in {nameofCurrentDomainUnhandledException}",
                    nameof(AppDomain.CurrentDomain.UnhandledException));
            };
        }

        protected override async void OnStartup(StartupEventArgs e)
        {
            IsDesignMode = false;
            base.OnStartup(e);
            await Host.StartAsync();
            Services.GetRequiredService<MainWindow>().Show();
        }

        protected override async void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
            using var host = Host;
            await host.StopAsync();
        }

        #endregion
    }
}

