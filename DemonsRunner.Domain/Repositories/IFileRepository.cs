namespace DemonsRunner.Domain.Repositories
{
    public interface IFileRepository<T>
    {
        public IEnumerable<T> GetAllFiles();

        public bool SaveFiles(IEnumerable<T> entities);
    }
}
