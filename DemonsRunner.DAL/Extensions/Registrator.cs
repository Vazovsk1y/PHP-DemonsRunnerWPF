using DemonsRunner.DAL.Repositories.Interfaces;
using DemonsRunner.DAL.Repositories;
using DemonsRunner.DAL.Storage.Interfaces;
using DemonsRunner.DAL.Storage;
using DemonsRunner.Domain.Models;
using Microsoft.Extensions.DependencyInjection;

namespace DemonsRunner.DAL.Extensions
{
    public static class Registrator
    {
        public static IServiceCollection AddDataAccessLayer(this IServiceCollection services) => services
            .AddScoped<IFileRepository<PHPDemon>, FileRepository>()
            .AddTransient<StorageFile>()
            .AddTransient<StorageDirectory>()
            .AddTransient<StorageResolver>(s => key =>
            {
                return key switch
                {
                    StorageType.Directory => s.GetRequiredService<StorageDirectory>(),
                    StorageType.File => s.GetRequiredService<StorageFile>(),
                    _ => throw new KeyNotFoundException(),
                };
            })
            ;
    }
}
