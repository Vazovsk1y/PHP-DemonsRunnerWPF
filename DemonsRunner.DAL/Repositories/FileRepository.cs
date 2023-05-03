using DemonsRunner.DAL.Storage;
using DemonsRunner.Domain.Models;
using DemonsRunner.Domain.Repositories;
using Newtonsoft.Json;

namespace DemonsRunner.DAL.Repositories
{
    public class FileRepository : IRepository<PHPDemon>
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

        public bool Save(IEnumerable<PHPDemon> items)
        {
            using var writer = new StreamWriter(_storageFile.FullPath);
            string json = JsonConvert.SerializeObject(items, Formatting.Indented);
            writer.Write(json);
            return true;
        }
    }
}
