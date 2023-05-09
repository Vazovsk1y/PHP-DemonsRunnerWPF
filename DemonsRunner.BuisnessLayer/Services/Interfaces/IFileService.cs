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
        public IResponse SaveAll(IEnumerable<PHPDemon> savedFiles);

        /// <summary>
        /// Checks is file exist on PC. 
        /// </summary>
        /// <returns>
        /// Response where Success status is Exist, and Fail is not Exist.
        /// </returns>
        public IResponse IsFileExist(PHPDemon file);
    }
}
