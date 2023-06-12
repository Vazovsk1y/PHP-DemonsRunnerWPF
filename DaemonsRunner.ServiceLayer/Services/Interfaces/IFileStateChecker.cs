using DaemonsRunner.BuisnessLayer.Responses.Interfaces;
using DaemonsRunner.Domain.Models;

namespace DaemonsRunner.BuisnessLayer.Services.Interfaces
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
