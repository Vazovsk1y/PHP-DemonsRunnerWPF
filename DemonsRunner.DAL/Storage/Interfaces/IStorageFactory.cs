namespace DemonsRunner.DAL.Storage.Interfaces
{
    public interface IStorageFactory
    {
        IStorage CreateStorage(StorageType storageType, string storageName);
    }

    public enum StorageType
    {
        File,
        Directory
    }
}
