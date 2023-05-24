using DemonsRunner.Domain.Models;
using DemonsRunner.Domain.Responses.Intefaces;

namespace DemonsRunner.BuisnessLayer.Services.Interfaces
{
    /// <summary>
    /// Service for interacting with storage file.
    /// </summary>
    public interface IFileService
    {
        /// <summary>
        /// Sends back data response, where data are files that store in storage file.
        /// </summary>
        public IDataResponse<IEnumerable<PHPDemon>> GetSaved();

        /// <summary>
        /// Sends back response with status operation, saves all files provided by.
        /// </summary>
        public IResponse SaveAll(IEnumerable<PHPDemon> saveFiles);
    }
}
