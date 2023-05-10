using DemonsRunner.Domain.Enums;
using DemonsRunner.Domain.Models;
using DemonsRunner.Domain.Responses.Intefaces;
using DemonsRunner.BuisnessLayer.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace DemonsRunner.BuisnessLayer.Services
{
    public class ScriptExecutorService : IScriptExecutorService
    {
        private readonly IFileService _fileService;
        private readonly ILogger<ScriptExecutorService> _logger;
        private readonly IResponseFactory _responseFactory;

        public ScriptExecutorService(IFileService fileService, ILogger<ScriptExecutorService> logger, IResponseFactory responseFactory)
        {
            _fileService = fileService;
            _logger = logger;
            _responseFactory = responseFactory;
        }

        public async Task<IDataResponse<PHPScriptExecutor>> StartAsync(PHPScript script, bool showExecutingWindow)
        {
            try
            {
                _logger.LogInformation("Starting [{scriptName}] started, show window - [{showExecutingWindow}]", script.Name, showExecutingWindow);
                var response = _fileService.IsFileExist(script.ExecutableFile);

                if (response.OperationStatus is StatusCode.Fail)
                {
                    _logger.LogError("The start was aborted, executable [{executableFileName}] file is not exist", script.ExecutableFile.Name);
                    return _responseFactory.CreateDataResponse<PHPScriptExecutor>(StatusCode.Fail, $"Executable {response.Description}");
                }

                var executor = new PHPScriptExecutor(script, showExecutingWindow);
                if (await executor.StartAsync().ConfigureAwait(false))
                {
                    _logger.LogInformation("Cmd was started successfully");
                    return _responseFactory.CreateDataResponse(StatusCode.Success, "Script was successfully started!", executor);
                }

                _logger.LogInformation("Cmd wasn't started");
                return _responseFactory.CreateDataResponse<PHPScriptExecutor>(StatusCode.Fail, "Script was NOT started!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{ExcetptionType} was catched", typeof(Exception));
                return _responseFactory.CreateDataResponse<PHPScriptExecutor>(StatusCode.Fail, "Something go wrong");
            }
        }

        public async Task<IResponse> StartMessagesReceivingAsync(PHPScriptExecutor runningScript)
        {
            try
            {
                _logger.LogInformation("Starting messages reception of [{runningScriptName}] has started", runningScript.ExecutableScript.Name);
                if (!runningScript.IsRunning)
                {
                    _logger.LogError("[{RunningScriptName}] was not running", runningScript.ExecutableScript.Name);
                    _responseFactory.CreateResponse(StatusCode.Fail, "Script was not running");
                }

                await runningScript.StartMessagesReceivingAsync().ConfigureAwait(false);
                _logger.LogInformation("Starting messages receiving was started successfully");
                return _responseFactory.CreateResponse(StatusCode.Success, "Messages receiving was started successfully");
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "{ExcetptionType} was catched", typeof(Exception));
                return _responseFactory.CreateResponse(StatusCode.Fail, "Something go wrong");
            }
        }

        public async Task<IResponse> ExecuteCommandAsync(PHPScriptExecutor runningScript)
        {
            try
            {
                _logger.LogInformation("Executing command in [{runningScriptName}] started", runningScript.ExecutableScript.Name);
                if (!runningScript.IsRunning)
                {
                    _logger.LogError("[{RunningScript}] was not running", runningScript.ExecutableScript.Name);
                    return _responseFactory.CreateResponse(StatusCode.Fail, "Script was not running");
                }

                await runningScript.ExecuteCommandAsync().ConfigureAwait(false);
                _logger.LogInformation("Executing command was completed successfully");
                return _responseFactory.CreateResponse(StatusCode.Success, "Command was executed successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{ExcetptionType} was catched", typeof(Exception));
                return _responseFactory.CreateResponse(StatusCode.Fail, "Something go wrong");
            }
        }

        public async Task<IResponse> StopAsync(PHPScriptExecutor executingScript)
        {
            try
            {
                _logger.LogInformation("Stopping script [{executingScriptName}] executing started.", executingScript.ExecutableScript.Name);
                if (!executingScript.IsRunning)
                {
                    _logger.LogInformation("[{executingScriptName}] was not running", executingScript.ExecutableScript.Name);
                    _responseFactory.CreateResponse(StatusCode.Fail, $"{executingScript.ExecutableScript.Name} is not running!");
                }

                await executingScript.StopAsync().ConfigureAwait(false);
                _logger.LogInformation("Cmd was killed successfully");
                return _responseFactory.CreateResponse(StatusCode.Success, "Script was killed successfully!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{ExcetptionType} was catched", typeof(Exception));
                return _responseFactory.CreateResponse(StatusCode.Fail, "Something go wrong");
            }
        }

        public async Task<IResponse> StopMessagesReceivingAsync(PHPScriptExecutor runningScript)
        {
            try
            {
                _logger.LogInformation("Stopping message receiving in [{runningScriptName}] started", runningScript.ExecutableScript.Name);
                if (!runningScript.IsRunning)
                {
                    _logger.LogError("[{RunningScript}] was not running", runningScript.ExecutableScript.Name);
                    return _responseFactory.CreateResponse(StatusCode.Fail, "Script was not running");
                }

                await runningScript.StopMessagesReceivingAsync().ConfigureAwait(false);
                _logger.LogInformation("Stopping message receiving successfully completed");
                return _responseFactory.CreateResponse(StatusCode.Success, "Message receiving was successfully stopped");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{ExcetptionType} was catched", typeof(Exception));
                return _responseFactory.CreateResponse(StatusCode.Fail, "Something go wrong");
            }
        }
    }
}
