using DemonsRunner.Domain.Models;
using DemonsRunner.Domain.Responses.Intefaces;

namespace DemonsRunner.BuisnessLayer.Services.Interfaces
{
    /// <summary>
    /// Service for configuring scripts.
    /// </summary>
    public interface IScriptConfigureService
    {
        /// <summary>
        /// Configure scripts according to files provided by.
        /// </summary>
        /// <returns>
        /// Data response, where data is all successfully configured scripts.
        /// </returns>
        public Task<IDataResponse<IEnumerable<PHPScript>>> ConfigureScripts(IEnumerable<PHPDemon> demons);
    }
}
