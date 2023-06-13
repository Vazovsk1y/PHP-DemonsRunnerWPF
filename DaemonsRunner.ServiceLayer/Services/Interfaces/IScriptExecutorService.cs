using DaemonsRunner.BuisnessLayer.Responses.Interfaces;
using DaemonsRunner.Domain.Models;

namespace DaemonsRunner.BuisnessLayer.Services.Interfaces
{
    /// <summary>
    /// Service for interracting with scripts executing.
    /// </summary>
    public interface IScriptExecutorService
    {
        /// <summary>
        /// Will create and run a new script execution model based on the passed configured script.
        /// </summary>
        /// <returns>
        /// Task from IDataResponse, where data is successfully started script execution model.
        /// </returns>
        public Task<IDataResponse<PHPScriptExecutor>> StartAsync(PHPScript script);

        /// <summary>
        /// Will stop execution of the running script execution model by killing its process within the operating system.
        /// </summary>
        /// <returns>
        /// IResponse with the operation status.
        /// </returns>
        public Task<IResponse> StopAsync(PHPScriptExecutor runningScriptExecutor);

        /// <summary>
        /// Begins asynchronus output reading from running script executor.
        /// </summary>
        /// <returns>
        /// IResponse with the operation status.
        /// </returns>
        public Task<IResponse> StartMessagesReceivingAsync(PHPScriptExecutor runningScriptExecutor);

        /// <summary>
        /// Executes the command in running script executor.
        /// </summary>
        /// <returns>
        /// IResponse with the operation status.
        /// </returns>
        public Task<IResponse> ExecuteCommandAsync(PHPScriptExecutor runningScriptExecutor);

        /// <summary>
        /// Breaks messages receiving in running script executor.
        /// </summary>
        /// <returns>
        /// IResponse with the operation status.
        /// </returns>
        public Task<IResponse> StopMessagesReceivingAsync(PHPScriptExecutor runningScriptExecutor);
    }
}
