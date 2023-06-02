using DemonsRunner.Domain.Models;
using DemonsRunner.Domain.Responses.Intefaces;

namespace DemonsRunner.BuisnessLayer.Services.Interfaces
{
    /// <summary>
    /// Service for configuring files into scripts.
    /// </summary>
    public interface IScriptConfigureService
    {
        /// <summary>
        /// Configures the scripts according to the files transferred.
        /// </summary>
        /// <returns>
        /// IDataResponse, where data is all successfully configured scripts.
        /// </returns>
        public Task<IDataResponse<IEnumerable<PHPScript>>> ConfigureScriptsAsync(IEnumerable<PHPFile> phpFiles);
    }
}
