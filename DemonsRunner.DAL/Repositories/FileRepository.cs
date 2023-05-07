using DemonsRunner.DAL.Repositories.Interfaces;
using DemonsRunner.DAL.Storage;
using DemonsRunner.Domain.Models;
using Newtonsoft.Json;

namespace DemonsRunner.DAL.Repositories
{
    public class FileRepository : IFileRepository<PHPDemon>
    {
        private readonly StorageFile _storageFile;

        public FileRepository(StorageFile storageFile)
        {
            _storageFile = storageFile;
        }

        public IEnumerable<PHPDemon> GetAll()
        {
            if (!File.Exists(_storageFile.FullPath))
            {
                return Enumerable.Empty<PHPDemon>();
            }
            using var reader = new StreamReader(_storageFile.FullPath);
            string json = reader.ReadToEnd();
            return JsonConvert.DeserializeObject<IEnumerable<PHPDemon>>(json);
        }

        public bool SaveAll(IEnumerable<PHPDemon> items)
        {
            using var writer = new StreamWriter(_storageFile.FullPath);
            string json = JsonConvert.SerializeObject(items, Formatting.Indented);
            writer.Write(json);
            return true;
        }
    }
}
