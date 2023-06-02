using DemonsRunner.DAL.Storage.Interfaces;

namespace DemonsRunner.DAL.Storage
{
    /// <summary>
    /// File that stores selected files(php-daemons) in previous app session.
    /// </summary>
    public class StorageFile : IStorage
    {
        // The class responsibility is to provide calling code the interface to interract with file on user pc.

        public string Name { get; }

        public string FullPath { get; }

        public StorageFile(IStorageFactory storageFactory, string fileName)
        {
            var storageDirectory = storageFactory.CreateStorage(StorageType.Directory, AppDomain.CurrentDomain.FriendlyName);
            Name = fileName;
            FullPath = Path.Combine(storageDirectory.FullPath, Name);

            if (!File.Exists(FullPath))
            {
                using var fileStream = File.Create(FullPath);
            }
        }
    }
}
