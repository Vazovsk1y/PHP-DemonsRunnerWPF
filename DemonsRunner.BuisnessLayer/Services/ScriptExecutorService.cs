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

        public async Task<IResponse> StartMessagesReceivingAsync(PHPScriptExecutor runningScript)
        {
            ArgumentNullException.ThrowIfNull(runningScript);
            _logger.LogInformation("Launching messages reception of [{runningScriptName}] has started", runningScript.ExecutableScript.Name);

            var response = new Response
            {
                OperationStatus = StatusCode.Fail,
            };

            if (!runningScript.IsRunning)
            {
                _logger.LogError("[{RunningScriptName}] wasn't started", runningScript.ExecutableScript.Name);
                response.Description = $"{runningScript.ExecutableScript.Name} script wasn't started messages receiving not available.";

                return response;
            }

            if (runningScript.IsMessagesReceivingEnable)
            {
                _logger.LogError("[{RunningScriptName}] messages receiving was already started", runningScript.ExecutableScript.Name);
                response.Description = $"{runningScript.ExecutableScript.Name} script messages receiving was already started.";

                return response;
            }

            await runningScript.StartMessagesReceivingAsync().ConfigureAwait(false);
            _logger.LogInformation("Launching messages receiving was completed successfully");
            response.Description = "Messages receiving was successfully started.";
            response.OperationStatus = StatusCode.Success;

            return response;
        }

        public async Task<IResponse> ExecuteCommandAsync(PHPScriptExecutor runningScript)
        {
            ArgumentNullException.ThrowIfNull(runningScript);
            _logger.LogInformation("Executing command in [{runningScriptName}] started", runningScript.ExecutableScript.Name);

            var response = new Response
            {
                OperationStatus = StatusCode.Fail,
            };

            if (!runningScript.IsRunning)
            {
                _logger.LogError("[{runningScriptName}] wasn't started", runningScript.ExecutableScript.Name);
                response.Description = $"{runningScript.ExecutableScript.Name} script wasn't started command executing canceled.";

                return response;
            }

            await runningScript.ExecuteCommandAsync().ConfigureAwait(false);
            _logger.LogInformation("Executing command was completed successfully");
            response.Description = "Command was successfully executed.";
            response.OperationStatus = StatusCode.Success;

            return response;
        }

        public async Task<IResponse> StopAsync(PHPScriptExecutor runningScript)
        {
            ArgumentNullException.ThrowIfNull(runningScript);
            _logger.LogInformation("Stopping script [{executingScriptName}] executing has started.", runningScript.ExecutableScript.Name);

            var response = new Response
            {
                OperationStatus = StatusCode.Fail,
            };

            if (!runningScript.IsRunning)
            {
                _logger.LogError("[{executingScriptName}] was not running", runningScript.ExecutableScript.Name);
                response.Description = $"{runningScript.ExecutableScript.Name} wasn't started no stopping is posible.";

                return response;
            }

            await runningScript.StopAsync().ConfigureAwait(false);
            _logger.LogInformation("Cmd was successfully killed");
            response.Description = $"{runningScript.ExecutableScript.Name} was successfully stopped!";
            response.OperationStatus = StatusCode.Success;

            return response;
        }

        public async Task<IResponse> StopMessagesReceivingAsync(PHPScriptExecutor runningScript)
        {
            ArgumentNullException.ThrowIfNull(runningScript);
            _logger.LogInformation("Stopping message receiving in [{runningScriptName}] has started", runningScript.ExecutableScript.Name);

            var response = new Response
            {
                OperationStatus = StatusCode.Fail,
            };

            if (!runningScript.IsRunning)
            {
                _logger.LogError("[{runningScriptName}] wasn't started", runningScript.ExecutableScript.Name);
                response.Description = 
                    $"{runningScript.ExecutableScript.Name} wasn't started stopping messages receiving is not available.";

                return response;
            }

            if (!runningScript.IsMessagesReceivingEnable)
            {
                _logger.LogError("[{RunningScriptName}] messages receiving wasn't started", runningScript.ExecutableScript.Name);
                response.Description = 
                    $"{runningScript.ExecutableScript.Name} script messages receiving wasn't started stopping messages receiving is not available.";

                return response;
            }

            await runningScript.StopMessagesReceivingAsync().ConfigureAwait(false);
            _logger.LogInformation("Stopping message receiving successfully completed");
            response.Description = "Messages receiving was successfully stopped.";
            response.OperationStatus = StatusCode.Success;

            return response;
        }
    }
}
