using DemonsRunner.BuisnessLayer.Responses;
using DemonsRunner.Domain.Enums;
using DemonsRunner.Domain.Interfaces;
using DemonsRunner.Domain.Models;
using DemonsRunner.Domain.Responses;
using DemonsRunner.Domain.Services;
using System.Diagnostics;

namespace DemonsRunner.BuisnessLayer.Services
{
    public class ScriptExecutorService : IScriptExecutorService
    {
        /// <summary>
        /// Start new cmd process, executing command and start message receiving from running process.
        /// </summary>
        public async Task<IDataResponse<PHPScriptExecutor>> StartExecutingAsync(PHPScript script, bool showExecutingWindow)
        {
            try
            {
                var executor = new PHPScriptExecutor(script, showExecutingWindow);
                if (await executor.StartAsync())
                {
                    await executor.ExecuteCommandAsync();
                    await executor.StartMessageReceivingAsync();

                    return new DataResponse<PHPScriptExecutor>
                    {
                        Description = "Script was successfully started!",
                        OperationStatus = StatusCode.Success,
                        Data = executor,
                    };
                }

                return new DataResponse<PHPScriptExecutor>
                {
                    Description = "Script was NOT started!",
                    OperationStatus = StatusCode.Fail,
                };
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return new DataResponse<PHPScriptExecutor>
                {
                    Description = "Something go wrong",
                    OperationStatus = StatusCode.Fail
                };
            }
        }

        public async Task<IBaseResponse> StopAsync(PHPScriptExecutor executingScript)
        {
            try
            {
                if (!executingScript.IsRunning)
                {
                    return new BaseResponse
                    {
                        Description = $"{executingScript.ExecutableScript.Name} is not running!",
                        OperationStatus = StatusCode.Fail
                    };
                }

                await executingScript.StopMessageReceivingAsync();
                await executingScript.StopAsync();
                return new BaseResponse
                {
                    Description = "Runner was killed successfully!",
                    OperationStatus = StatusCode.Success,
                };
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message + "\n" + ex.Source);
                return new BaseResponse
                {
                    Description = "Something go wrong",
                    OperationStatus = StatusCode.Fail
                };
            }
        }
    }
}
