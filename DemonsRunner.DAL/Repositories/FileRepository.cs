using DemonsRunner.Domain.Models;
using DemonsRunner.Domain.Repositories;
using Newtonsoft.Json;

namespace DemonsRunner.DAL.Repositories
{
    public class FileRepository : IFileRepository<PHPDemon>
    {
        private readonly string _filePath;

        public FileRepository()
        {
            string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string appName = "DemonsRunner";
            string directoryPath = Path.Combine(appDataPath, appName);

            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }
            _filePath = Path.Combine(directoryPath, "data.json");
        }

        public IEnumerable<PHPDemon> GetAllFiles()
        {
            if (!File.Exists(_filePath))
            {
                return Enumerable.Empty<PHPDemon>();
            }
            using StreamReader reader = new StreamReader(_filePath);
            string json = reader.ReadToEnd();
            return JsonConvert.DeserializeObject<IEnumerable<PHPDemon>>(json);
        }

        public bool SaveFiles(IEnumerable<PHPDemon> items)
        {
            using StreamWriter writer = new StreamWriter(_filePath);
            string json = JsonConvert.SerializeObject(items, Formatting.Indented);
            writer.Write(json);
            return true;
        }
    }
}
