using System.Reflection;

namespace DemonsRunner.DAL.Storage
{
    internal class StorageFile
    {
        private readonly string _name;

        private readonly string _fullPath;

        public string Name => _name;

        public string FullPath => _fullPath;

        public StorageFile(string fileName)
        {
            _name = fileName;
            string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string appName = Assembly.GetEntryAssembly().GetName().Name;
            string fileDirectoryPath = Path.Combine(appDataPath, appName);

            if (!Directory.Exists(fileDirectoryPath)) 
            {
                Directory.CreateDirectory(fileDirectoryPath);
            }
            _fullPath = Path.Combine(fileDirectoryPath, Name);
        }
    }
}
