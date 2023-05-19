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
            .AddTransient<IStorageFile, StorageFile>(p => new StorageFile("data.json"))
            ;
    }
}
