using DemonsRunner.DAL.Storage.Interfaces;
using System.Reflection;

namespace DemonsRunner.DAL.Storage
{
    /// <summary>
    /// File in AppData folder that store the selected files(php-daemons) on a previous app session.
    /// </summary>
    public class StorageFile : IStorageFile
    {
        private readonly string _name;

        private readonly string _fullPath;

        public string Name => _name;

        public string FullPath => _fullPath;

        public StorageFile(string fileName)
        {
            _name = fileName;
            string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string appName = AppDomain.CurrentDomain.FriendlyName;
            string fileDirectoryPath = Path.Combine(appDataPath, appName);

            if (!Directory.Exists(fileDirectoryPath)) 
            {
                Directory.CreateDirectory(fileDirectoryPath);
            }
            _fullPath = Path.Combine(fileDirectoryPath, Name);
        }
    }
}
