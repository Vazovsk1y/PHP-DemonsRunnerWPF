using DemonsRunner.DAL.Storage.Interfaces;

namespace DemonsRunner.DAL.Storage
{
    internal class StorageFactory : IStorageFactory
    {
        // The class responsibility is to create a particular storage instance depends on passed arguments.

        public IStorage CreateStorage(StorageType storageType, string storageName)
        {
            return storageType switch
            {
                StorageType.File => new StorageFile(this, storageName),
                StorageType.Directory => new StorageDirectory(storageName),
                _ => throw new KeyNotFoundException(),
            };
        }
    }
}
