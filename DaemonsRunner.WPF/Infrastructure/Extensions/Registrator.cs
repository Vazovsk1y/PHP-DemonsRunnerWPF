using DaemonsRunner.BuisnessLayer.Services.Interfaces;
using DaemonsRunner.Infrastructure.Managers;
using DaemonsRunner.Infrastructure.Managers.Interfaces;
using DaemonsRunner.Services;
using DaemonsRunner.ViewModels;
using DaemonsRunner.ViewModels.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace DaemonsRunner.Infrastructure.Extensions
{
    internal static class Registrator
    {
        public static IServiceCollection AddClientLayer(this IServiceCollection services) => services
            .AddSingleton<IScriptExecutorViewModelFactory, PHPScriptExecutorViewModelFactory>()
            .AddSingleton<MainWindowViewModel>()
            .AddSingleton<FilesPanelViewModel>()
            .AddSingleton<WorkSpaceViewModel>()
            .AddSingleton<NotificationPanelViewModel>()
            .AddSingleton<IServiceManager, ServiceManager>()
            .AddTransient<IFileDialog, WPFFileDialogService>()
            .AddSingleton(s =>
            {
                var viewModel = s.GetRequiredService<MainWindowViewModel>();
                var window = new MainWindow { DataContext = viewModel };

                return window;
            })
            ;
    }
}
