using DemonsRunner.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace DemonsRunner.Infrastructure.Extensions
{
    internal static class Registrator
    {
        public static IServiceCollection AddClientLayer(this IServiceCollection services) => services
            .AddSingleton<IScriptExecutorViewModelFactory, PHPScriptExecutorViewModelFactory>()
            .AddSingleton<MainWindowViewModel>()
            .AddSingleton<FilesPanelViewModel>()
            .AddSingleton<WorkSpaceViewModel>()
            .AddSingleton<NotificationPanelViewModel>()
            .AddSingleton(s =>
            {
                var viewModel = s.GetRequiredService<MainWindowViewModel>();
                var window = new MainWindow { DataContext = viewModel };

                return window;
            })
            ;
    }
}
