using DemonsRunner.BuisnessLayer.Responses.Interfaces;
using DemonsRunner.Domain.Models;

namespace DemonsRunner.BuisnessLayer.Services.Interfaces
{
    /// <summary>
    /// Service for interacting with file repository.
    /// </summary>
    public interface IFileService
    {
        /// <summary>
        /// Returns back data response, where data are files that store in storage file.
        /// </summary>
        public IDataResponse<IEnumerable<PHPFile>> GetSaved();

        /// <summary>
        /// Saves all files provided by.
        /// </summary>
        /// <returns>
        /// Returns response with saving files operation result status.
        /// </returns>
        public IResponse SaveAll(IEnumerable<PHPFile> saveFiles);
    }
}
