using DemonsRunner.DAL.Storage.Interfaces;
using System.Reflection;

namespace DemonsRunner.DAL.Storage
{
    /// <summary>
    /// File in AppData folder that store the selected files(php-daemons) on a previous app session.
    /// </summary>
    public class StorageFile : IStorage
    {
        public string Name => "data.json";

        public string FullPath { get; }

        public StorageFile(StorageResolver resolver)
        {
            var storageDirectory = resolver.Invoke(StorageType.Directory);
            FullPath = Path.Combine(storageDirectory.FullPath, Name);

            if (!File.Exists(FullPath))
            {
                using var fileStream = File.Create(FullPath);
            }
        }
    }
}
