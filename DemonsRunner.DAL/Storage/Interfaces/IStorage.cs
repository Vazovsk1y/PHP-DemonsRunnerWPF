namespace DemonsRunner.DAL.Storage.Interfaces
{
    public interface IStorage
    {
        string Name { get; }

        string FullPath { get; }
    }

    public enum StorageType
    {
        File,
        Directory
    }

    public delegate IStorage StorageResolver(StorageType type);
}
