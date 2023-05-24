using DemonsRunner.BuisnessLayer.Services.Interfaces;
using DemonsRunner.BuisnessLayer.Services;
using Microsoft.Extensions.DependencyInjection;

namespace DemonsRunner.BuisnessLayer.Extensions
{
    public static class Registrator
    {
        public static IServiceCollection AddBuisnessLayer(this IServiceCollection services) => services
            .AddTransient<IFileService, FileService>()
            .AddTransient<IFileDialogService, FileDialogService>()
            .AddTransient<IScriptConfigureService, ScriptConfigureService>()
            .AddTransient<IScriptExecutorService, ScriptExecutorService>()
            .AddTransient<IFileStateChecker, FileStateChecker>()
            .AddSingleton<IDataBus, DataBusService>();
    }
}
