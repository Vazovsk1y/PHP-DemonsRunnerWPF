using DemonsRunner.DAL.Repositories.Interfaces;
using DemonsRunner.DAL.Storage.Interfaces;
using DemonsRunner.Domain.Models;
using Newtonsoft.Json;

namespace DemonsRunner.DAL.Repositories
{
    public class FileRepository : IFileRepository<PHPFile>
    {
        // The class responsibility is provide to calling code the interface to interract with data in storage json file on user pc.
        // Analogy of the repository pattern at work with dbContext of EF.

        private readonly IStorage _storageFile;
        private readonly object _locker = new object();

        public FileRepository(IStorageFactory storageFactory)
        {
            _storageFile = storageFactory.CreateStorage(StorageType.File, "data.json");
        }

        public IEnumerable<PHPFile> GetAll()
        {
            if (!File.Exists(_storageFile.FullPath))
            {
                throw new InvalidOperationException("The storage file has been deleted or renamed.");
            }

            lock(_locker)
            {
                using var reader = new StreamReader(_storageFile.FullPath);
                string json = reader.ReadToEnd();
                return JsonConvert.DeserializeObject<IEnumerable<PHPFile>>(json) ?? Enumerable.Empty<PHPFile>();
            }
        }

        public void SaveAll(IEnumerable<PHPFile> items)
        {
            lock (_locker)
            {
                using var writer = new StreamWriter(_storageFile.FullPath);
                string json = JsonConvert.SerializeObject(items, Formatting.Indented);
                writer.Write(json);
            }
        }
    }
}
