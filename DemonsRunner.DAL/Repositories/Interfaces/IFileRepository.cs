namespace DemonsRunner.DAL.Repositories.Interfaces
{
    /// <summary>
    /// Repository to interract with storage file.
    /// </summary>
    public interface IFileRepository<T> where T : class
    {
        /// <summary>
        /// Provides you all files that stores in storage file.
        /// </summary>
        public IEnumerable<T> GetAll();

        /// <summary>
        /// Saves all provided by you files in storage file.
        /// </summary>
        public void SaveAll(IEnumerable<T> entities);
    }
}
