using DemonsRunner.DAL.Storage.Interfaces;

namespace DemonsRunner.DAL.Storage
{
    public class StorageDirectory : IStorage
    {
        // The class responsibility is to provide calling code the interface to interract with directory on user pc.

        public string FullPath { get; }

        public string Name { get; }

        public StorageDirectory(string directoryName)
        {
            Name = directoryName;
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
