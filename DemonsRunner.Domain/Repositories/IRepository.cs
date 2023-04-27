namespace DemonsRunner.Domain.Repositories
{
    public interface IRepository<T>
    {
        public IEnumerable<T> GetAll();

        public void Save(IEnumerable<T> entities);
    }
}
