using DaemonsRunner.BuisnessLayer.Services.Interfaces;
using DaemonsRunner.BuisnessLayer.Services;
using Microsoft.Extensions.DependencyInjection;

namespace DaemonsRunner.BuisnessLayer.Extensions
{
    public static class Registrator
    {
        public static IServiceCollection AddBuisnessLayer(this IServiceCollection services) => services
            .AddTransient<IFileService, FileService>()
            .AddTransient<IScriptConfigureService, ScriptConfigureService>()
            .AddTransient<IScriptExecutorService, ScriptExecutorService>()
            .AddTransient<IFileStateChecker, FileStateChecker>()
            .AddSingleton<IDataBus, DataBusService>();
    }
}
