using DemonsRunner.Domain.Enums;
using DemonsRunner.Domain.Models;
using DemonsRunner.Domain.Responses.Intefaces;
using DemonsRunner.BuisnessLayer.Services.Interfaces;
using Microsoft.Extensions.Logging;
using DemonsRunner.Domain.Responses;

namespace DemonsRunner.BuisnessLayer.Services
{
    public class ScriptExecutorService : IScriptExecutorService
    {
        private readonly IFileStateChecker _fileStateChecker;
        private readonly ILogger<ScriptExecutorService> _logger;

        public ScriptExecutorService(
            ILogger<ScriptExecutorService> logger, 
            IFileStateChecker fileStateChecker)
        {
            _logger = logger;
            _fileStateChecker = fileStateChecker;
        } 

        public async Task<IDataResponse<PHPScriptExecutor>> StartAsync(PHPScript script)
        {
            ArgumentNullException.ThrowIfNull(script);

            _logger.LogInformation("Launching [{scriptName}] started", script.Name);
            var response = new DataResponse<PHPScriptExecutor> 
            {
                OperationStatus = StatusCode.Fail,
            };

            var fileStateCheckerResponse = await _fileStateChecker.IsFileExistAsync(script.ExecutableFile).ConfigureAwait(false);
            if (fileStateCheckerResponse.OperationStatus is StatusCode.Fail)
            {
                _logger.LogWarning("The start was aborted, executable [{executableFileName}] file is not exist", script.ExecutableFile.Name);
                response.Description = $"{fileStateCheckerResponse.Description} The launch was canceled.";

                return response;
            }

            var executor = new PHPScriptExecutor(script);
            if (await executor.StartAsync().ConfigureAwait(false))
            {
                _logger.LogInformation("Cmd was successfully started");
                response.Description = "Script was successfully started.";
                response.OperationStatus = StatusCode.Success;
                response.Data = executor;

                return response;
            }

            _logger.LogError("Cmd wasn't started");
            response.Description = "Script wasn't started.";

            return response;
        }

        public async Task<IResponse> StartMessagesReceivingAsync(PHPScriptExecutor runningScriptExecutor)
        {
            ArgumentNullException.ThrowIfNull(runningScriptExecutor);

            _logger.LogInformation("Launching messages reception of [{runningScriptName}] has started", runningScriptExecutor.ExecutableScript.Name);
            var response = new Response
            {
                OperationStatus = StatusCode.Fail,
            };

            if (!runningScriptExecutor.IsRunning)
            {
                _logger.LogError("[{RunningScriptName}] wasn't started", runningScriptExecutor.ExecutableScript.Name);
                response.Description = $"{runningScriptExecutor.ExecutableScript.Name} script wasn't started messages receiving not available.";

                return response;
            }

            if (runningScriptExecutor.IsMessagesReceivingEnable)
            {
                _logger.LogError("[{RunningScriptName}] messages receiving was already started", runningScriptExecutor.ExecutableScript.Name);
                response.Description = $"{runningScriptExecutor.ExecutableScript.Name} script messages receiving was already started.";

                return response;
            }

            await runningScriptExecutor.StartMessagesReceivingAsync().ConfigureAwait(false);
            _logger.LogInformation("Launching messages receiving was completed successfully");
            response.Description = "Messages receiving was successfully started.";
            response.OperationStatus = StatusCode.Success;

            return response;
        }

        public async Task<IResponse> ExecuteCommandAsync(PHPScriptExecutor runningScriptExecutor)
        {
            ArgumentNullException.ThrowIfNull(runningScriptExecutor);

            _logger.LogInformation("Executing command in [{runningScriptName}] started", runningScriptExecutor.ExecutableScript.Name);
            var response = new Response
            {
                OperationStatus = StatusCode.Fail,
            };

            if (!runningScriptExecutor.IsRunning)
            {
                _logger.LogError("[{runningScriptName}] wasn't started", runningScriptExecutor.ExecutableScript.Name);
                response.Description = $"{runningScriptExecutor.ExecutableScript.Name} script wasn't started command executing canceled.";

                return response;
            }

            await runningScriptExecutor.ExecuteCommandAsync().ConfigureAwait(false);
            _logger.LogInformation("Executing command was completed successfully");
            response.Description = "Command was successfully executed.";
            response.OperationStatus = StatusCode.Success;

            return response;
        }

        public async Task<IResponse> StopAsync(PHPScriptExecutor runningScriptExecutor)
        {
            ArgumentNullException.ThrowIfNull(runningScriptExecutor);

            _logger.LogInformation("Stopping script [{executingScriptName}] executing has started.", runningScriptExecutor.ExecutableScript.Name);
            var response = new Response
            {
                OperationStatus = StatusCode.Fail,
            };

            if (!runningScriptExecutor.IsRunning)
            {
                _logger.LogError("[{executingScriptName}] was not running", runningScriptExecutor.ExecutableScript.Name);
                response.Description = $"{runningScriptExecutor.ExecutableScript.Name} wasn't started no stopping is posible.";

                return response;
            }

            await runningScriptExecutor.StopAsync().ConfigureAwait(false);
            _logger.LogInformation("Cmd was successfully killed");
            response.Description = $"{runningScriptExecutor.ExecutableScript.Name} was successfully stopped!";
            response.OperationStatus = StatusCode.Success;

            return response;
        }

        public async Task<IResponse> StopMessagesReceivingAsync(PHPScriptExecutor runningScriptExecutor)
        {
            ArgumentNullException.ThrowIfNull(runningScriptExecutor);

            _logger.LogInformation("Stopping message receiving in [{runningScriptName}] has started", runningScriptExecutor.ExecutableScript.Name);
            var response = new Response
            {
                OperationStatus = StatusCode.Fail,
            };

            if (!runningScriptExecutor.IsRunning)
            {
                _logger.LogError("[{runningScriptName}] wasn't started", runningScriptExecutor.ExecutableScript.Name);
                response.Description = 
                    $"{runningScriptExecutor.ExecutableScript.Name} wasn't started stopping messages receiving is not available.";

                return response;
            }

            if (!runningScriptExecutor.IsMessagesReceivingEnable)
            {
                _logger.LogError("[{RunningScriptName}] messages receiving wasn't started", runningScriptExecutor.ExecutableScript.Name);
                response.Description = 
                    $"{runningScriptExecutor.ExecutableScript.Name} script messages receiving wasn't started stopping messages receiving is not available.";

                return response;
            }

            await runningScriptExecutor.StopMessagesReceivingAsync().ConfigureAwait(false);
            _logger.LogInformation("Stopping message receiving successfully completed");
            response.Description = "Messages receiving was successfully stopped.";
            response.OperationStatus = StatusCode.Success;

            return response;
        }
    }
}
