using DemonsRunner.BuisnessLayer.Responses.Interfaces;
using DemonsRunner.Domain.Models;

namespace DemonsRunner.BuisnessLayer.Services.Interfaces
{
    public interface IFileStateChecker
    {
        /// <summary>
        /// Checks is file exist on PC. 
        /// </summary>
        /// <returns>
        /// Response where Success status is Exist, and Fail is not Exist.
        /// </returns>
        Task<IResponse> IsFileExistAsync(PHPFile file);
    }
}
