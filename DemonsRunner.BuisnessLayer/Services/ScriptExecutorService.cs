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
        public Task<IDataResponse<PHPScriptExecutor>> StartAsync(PHPScript script, bool showExecutingWindow)
        {
            try
            {
                var executor = new PHPScriptExecutor(script, showExecutingWindow);
                if (executor.Start())
                {
                    executor.StartMessageReceiving();
                    executor.ExecuteCommand();
                    return Task.FromResult <IDataResponse<PHPScriptExecutor>>(new DataResponse<PHPScriptExecutor>
                    {
                        Description = "Script was successfully started!",
                        OperationStatus = StatusCode.Success,
                        Data = executor,
                    });
                }

                return Task.FromResult<IDataResponse<PHPScriptExecutor>>(new DataResponse<PHPScriptExecutor>
                {
                    Description = "Script was NOT started!",
                    OperationStatus = StatusCode.Fail,
                });

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return Task.FromResult<IDataResponse<PHPScriptExecutor>>(new DataResponse<PHPScriptExecutor>
                {
                    Description = "Something go wrong",
                    OperationStatus = StatusCode.Fail
                });
            }
        }

        public Task<IBaseResponse> StopAsync(IEnumerable<PHPScriptExecutor> executingScripts)
        {
            try
            {
                foreach (var executingScript in executingScripts)
                {
                    if (!executingScript.IsRunning)
                    {
                        return Task.FromResult<IBaseResponse>(new BaseResponse
                        {
                            Description = $"{executingScript.ExecutableScript.Name} is not running!",
                            OperationStatus = StatusCode.Fail
                        });
                    }
                    executingScript.Stop();
                    executingScript.Dispose();
                }

                return Task.FromResult<IBaseResponse>(new BaseResponse
                {
                    Description = "Runners were killed and disposed successfully!",
                    OperationStatus = StatusCode.Success,
                });
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return Task.FromResult<IBaseResponse>(new BaseResponse
                {
                    Description = "Something go wrong",
                    OperationStatus = StatusCode.Fail
                });
            }
        }
    }
}
