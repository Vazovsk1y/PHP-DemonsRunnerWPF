using DemonsRunner.DAL.Storage.Interfaces;

namespace DemonsRunner.DAL.Storage
{
    public class StorageDirectory : IStorage
    {
        public string FullPath { get; }

        public string Name => AppDomain.CurrentDomain.FriendlyName;

        public StorageDirectory()
        {
            string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string storageDirectoryPath = Path.Combine(appDataPath, Name);
            FullPath = storageDirectoryPath;

            if (!Directory.Exists(storageDirectoryPath))
            {
                Directory.CreateDirectory(storageDirectoryPath);
            }
        }
    }
}
