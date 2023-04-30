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
        public IDataResponse<PHPScriptExecutor> Start(PHPScript script, bool showExecutingWindow)
        {
            try
            {
                var executor = new PHPScriptExecutor(script, showExecutingWindow);
                if (executor.Start())
                {
                    return new DataResponse<PHPScriptExecutor>
                    {
                        Description = "Scripts were successfully started!",
                        Data = executor,
                        OperationStatus = StatusCode.Success
                    };
                }

                return new DataResponse<PHPScriptExecutor>
                {
                    Description = "Something go wrong",
                    OperationStatus = StatusCode.Fail
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

        public IBaseResponse Stop(IEnumerable<PHPScriptExecutor> executingScripts)
        {
            try
            {
                foreach (var executingScript in executingScripts)
                {
                    if (!executingScript.IsRunning)
                    {
                        return new BaseResponse
                        {
                            Description = $"Some {executingScript.ExecutableScript.Name} is not running!",
                            OperationStatus = StatusCode.Fail
                        };
                    }
                    executingScript.Stop();
                    executingScript.Dispose();
                }
             
                return new BaseResponse
                {
                    OperationStatus = StatusCode.Success,
                    Description = "Runners were killed and disposed successfully!",
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
    }
}
