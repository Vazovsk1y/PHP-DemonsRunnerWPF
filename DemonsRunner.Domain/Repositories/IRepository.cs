namespace DemonsRunner.Domain.Repositories
{
    public interface IRepository<T> where T : class
    {
        public IEnumerable<T> GetAll();

        public bool Save(IEnumerable<T> entities);
    }
}
