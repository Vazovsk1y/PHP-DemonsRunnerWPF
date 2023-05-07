using DemonsRunner.Domain.Models;
using DemonsRunner.Domain.Responses.Intefaces;

namespace DemonsRunner.BuisnessLayer.Services.Interfaces
{
    /// <summary>
    /// Service for interaction with script executing.
    /// </summary>
    public interface IScriptExecutorService
    {
        /// <summary>
        /// Starts new cmd process, begins receiving messages and executes the command that stores in configured script provided by.
        /// </summary>
        /// <returns>
        /// Task from data response, where data is successfully started script.
        /// </returns>
        public Task<IDataResponse<PHPScriptExecutor>> LaunchAsync(PHPScript script, bool showExecutingWindow);

        /// <summary>
        /// Breaks message receiving and kills the cmd process in running script.
        /// </summary>
        public Task<IResponse> StopAsync(PHPScriptExecutor executingScript);
    }
}
