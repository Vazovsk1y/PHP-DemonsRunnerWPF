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
        /// Starts new cmd process with configured script provided by.
        /// </summary>
        /// <returns>
        /// Task from data response, where data is successfully started script.
        /// </returns>
        public Task<IDataResponse<PHPScriptExecutor>> StartAsync(PHPScript script, bool showExecutingWindow);

        /// <summary>
        /// Kills the running cmd process in executing script.
        /// </summary>
        public Task<IResponse> StopAsync(PHPScriptExecutor executingScript);

        /// <summary>
        /// Begins asynchronus output reading from running cmd in executing script.
        /// </summary>
        public Task<IResponse> StartMessagesReceivingAsync(PHPScriptExecutor runningScript);

        /// <summary>
        /// Executes the command in running cmd of executing script.
        /// </summary>
        public Task<IResponse> ExecuteCommandAsync(PHPScriptExecutor runningScript);

        /// <summary>
        /// Breaks message receiving from running cmd in executing script.
        /// </summary>
        public Task<IResponse> StopMessagesReceivingAsync(PHPScriptExecutor runningScript);
    }
}
