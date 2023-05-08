using DemonsRunner.Domain.Responses;
using DemonsRunner.Domain.Enums;
using DemonsRunner.Domain.Models;
using DemonsRunner.Domain.Responses.Intefaces;
using System.Diagnostics;
using DemonsRunner.BuisnessLayer.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace DemonsRunner.BuisnessLayer.Services
{
    public class ScriptExecutorService : IScriptExecutorService
    {
        private readonly IFileService _fileService;
        private readonly ILogger<ScriptExecutorService> _logger;

        public ScriptExecutorService(IFileService fileService, ILogger<ScriptExecutorService> logger) 
        {
            _fileService = fileService;
            _logger = logger;
        }

        public async Task<IDataResponse<PHPScriptExecutor>> LaunchAsync(PHPScript script, bool showExecutingWindow)
        {
            try
            {
                _logger.LogInformation("Launching {scriptName} started, show window - {showExecutingWindow}", script.Name, showExecutingWindow);
                var response = _fileService.IsFileExist(script.ExecutableFile);

                if (response.OperationStatus is StatusCode.Fail)
                {
                    _logger.LogError("The launch was aborted, executable file is not exist");
                    return new DataResponse<PHPScriptExecutor>
                    {
                        Description = $"{response.Description}",
                        OperationStatus = StatusCode.Fail,
                    };
                }

                var executor = new PHPScriptExecutor(script, showExecutingWindow);
                if (await executor.StartAsync())
                {
                    _logger.LogInformation("Cmd was started successfully");
                    await executor.ExecuteCommandAsync();
                    _logger.LogInformation("Command was executed successfully");
                    await executor.StartMessageReceivingAsync();
                    _logger.LogInformation("Message receiving was started successfully");

                    return new DataResponse<PHPScriptExecutor>
                    {
                        Description = "Script was successfully started!",
                        OperationStatus = StatusCode.Success,
                        Data = executor,
                    };
                }

                _logger.LogInformation("Cmd wasn't started");
                return new DataResponse<PHPScriptExecutor>
                {
                    Description = "Script was NOT started!",
                    OperationStatus = StatusCode.Fail,
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{ExcetptionType} was catched", typeof(Exception));
                return new DataResponse<PHPScriptExecutor>
                {
                    Description = "Something go wrong",
                    OperationStatus = StatusCode.Fail
                };
            }
        }

        public async Task<IResponse> StopAsync(PHPScriptExecutor executingScript)
        {
            try
            {
                _logger.LogInformation("Stopping {executingScriptName} script started", executingScript.ExecutableScript.Name);
                if (!executingScript.IsRunning)
                {
                    _logger.LogInformation("{executingScriptName} was not running", executingScript.ExecutableScript.Name);
                    return new Response
                    {
                        Description = $"{executingScript.ExecutableScript.Name} is not running!",
                        OperationStatus = StatusCode.Fail
                    };
                }

                await executingScript.StopMessageReceivingAsync();
                _logger.LogInformation("Stopping message receiving successfully completed");
                await executingScript.StopAsync();
                _logger.LogInformation("Cmd was killed successfully");

                return new Response
                {
                    Description = "Runner was killed successfully!",
                    OperationStatus = StatusCode.Success,
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{ExcetptionType} was catched", typeof(Exception));
                return new Response
                {
                    Description = "Something go wrong",
                    OperationStatus = StatusCode.Fail
                };
            }
        }
    }
}
