using DaemonsRunner.DAL.Repositories.Interfaces;
using DaemonsRunner.DAL.Repositories;
using DaemonsRunner.DAL.Storage.Interfaces;
using DaemonsRunner.DAL.Storage;
using DaemonsRunner.Domain.Models;
using Microsoft.Extensions.DependencyInjection;

namespace DaemonsRunner.DAL.Extensions
{
    public static class Registrator
    {
        public static IServiceCollection AddDataAccessLayer(this IServiceCollection services) => services
            .AddScoped<IFileRepository<PHPFile>, FileRepository>()
            .AddTransient<IStorageFactory, StorageFactory>()
            ;
    }
}
