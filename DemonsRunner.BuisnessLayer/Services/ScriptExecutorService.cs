using DemonsRunner.Domain.Enums;
using DemonsRunner.Domain.Models;
using DemonsRunner.Domain.Responses.Intefaces;
using DemonsRunner.BuisnessLayer.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace DemonsRunner.BuisnessLayer.Services
{
    public class ScriptExecutorService : IScriptExecutorService
    {
        private readonly IFileStateChecker _fileStateChecker;
        private readonly ILogger<ScriptExecutorService> _logger;
        private readonly IResponseFactory _responseFactory;

        public ScriptExecutorService(
            ILogger<ScriptExecutorService> logger, 
            IResponseFactory responseFactory, 
            IFileStateChecker fileStateChecker)
        {
            _logger = logger;
            _responseFactory = responseFactory;
            _fileStateChecker = fileStateChecker;
        }

        public async Task<IDataResponse<PHPScriptExecutor>> StartAsync(PHPScript script, bool showExecutingWindow)
        {
            string messageResponse = string.Empty;
            try
            {
                _logger.LogInformation("Starting [{scriptName}] started, show window - [{showExecutingWindow}]", script.Name, showExecutingWindow);
                var response = await _fileStateChecker.IsFileExistAsync(script.ExecutableFile).ConfigureAwait(false);

                if (response.OperationStatus is StatusCode.Fail)
                {
                    _logger.LogError("The start was aborted, executable [{executableFileName}] file is not exist", script.ExecutableFile.Name);
                    messageResponse = $"{response.Description} the launch was canceled";

                    return _responseFactory.CreateDataResponse<PHPScriptExecutor>(StatusCode.Fail, messageResponse);
                }

                var executor = new PHPScriptExecutor(script, showExecutingWindow);
                if (await executor.StartAsync().ConfigureAwait(false))
                {
                    _logger.LogInformation("Cmd was started successfully");
                    messageResponse = "Script was successfully started";

                    return _responseFactory.CreateDataResponse(StatusCode.Success, messageResponse, executor);
                }

                _logger.LogError("Cmd wasn't started");
                messageResponse = "Script wasn't started.";

                return _responseFactory.CreateDataResponse<PHPScriptExecutor>(StatusCode.Fail, messageResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{ExcetptionType} was catched", typeof(Exception));
                messageResponse = "Something go wrong";

                return _responseFactory.CreateDataResponse<PHPScriptExecutor>(StatusCode.Fail, messageResponse);
            }
        }

        public async Task<IResponse> StartMessagesReceivingAsync(PHPScriptExecutor runningScript)
        {
            string messageResponse = string.Empty;
            try
            {
                _logger.LogInformation("Starting messages reception of [{runningScriptName}] has started", runningScript.ExecutableScript.Name);
                if (!runningScript.IsRunning)
                {
                    _logger.LogError("[{RunningScriptName}] was not running", runningScript.ExecutableScript.Name);
                    messageResponse = $"{runningScript.ExecutableScript.Name} script was not running messages receiving not available.";

                    return _responseFactory.CreateResponse(StatusCode.Fail, messageResponse);
                }

                if (runningScript.IsMessagesReceiving)
                {
                    _logger.LogError("[{RunningScriptName}] messages receiving was already started", runningScript.ExecutableScript.Name);
                    messageResponse = $"{runningScript.ExecutableScript.Name} script messages receiving was already started";

                    return _responseFactory.CreateResponse(StatusCode.Fail, messageResponse);
                }

                await runningScript.StartMessagesReceivingAsync().ConfigureAwait(false);
                _logger.LogInformation("Starting messages receiving was started successfully");
                messageResponse = "Messages receiving was started successfully";

                return _responseFactory.CreateResponse(StatusCode.Success, messageResponse);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "{ExcetptionType} was catched", typeof(Exception));
                messageResponse = "Something go wrong";

                return _responseFactory.CreateResponse(StatusCode.Fail, messageResponse);
            }
        }

        public async Task<IResponse> ExecuteCommandAsync(PHPScriptExecutor runningScript)
        {
            string messageResponse = string.Empty;
            try
            {
                _logger.LogInformation("Executing command in [{runningScriptName}] started", runningScript.ExecutableScript.Name);
                if (!runningScript.IsRunning)
                {
                    _logger.LogError("[{runningScriptName}] was not running", runningScript.ExecutableScript.Name);
                    messageResponse = $"{runningScript.ExecutableScript.Name} script was not running command executing canceled.";

                    return _responseFactory.CreateResponse(StatusCode.Fail, messageResponse);
                }

                await runningScript.ExecuteCommandAsync().ConfigureAwait(false);
                _logger.LogInformation("Executing command was completed successfully");
                messageResponse = "Command was executed successfully";

                return _responseFactory.CreateResponse(StatusCode.Success, messageResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{ExcetptionType} was catched", typeof(Exception));
                messageResponse = "Something go wrong";

                return _responseFactory.CreateResponse(StatusCode.Fail, messageResponse);
            }
        }

        public async Task<IResponse> StopAsync(PHPScriptExecutor executingScript)
        {
            string messageResponse = string.Empty;
            try
            {
                _logger.LogInformation("Stopping script [{executingScriptName}] executing started.", executingScript.ExecutableScript.Name);
                if (!executingScript.IsRunning)
                {
                    _logger.LogInformation("[{executingScriptName}] was not running", executingScript.ExecutableScript.Name);
                    messageResponse = $"{executingScript.ExecutableScript.Name} is not running stoping is imposible.";

                    return _responseFactory.CreateResponse(StatusCode.Fail, messageResponse);
                }

                await executingScript.StopAsync().ConfigureAwait(false);
                _logger.LogInformation("Cmd was killed successfully");
                messageResponse = $"{executingScript.ExecutableScript.Name} was stopped successfully!";

                return _responseFactory.CreateResponse(StatusCode.Success, messageResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{ExcetptionType} was catched", typeof(Exception));
                messageResponse = "Something go wrong";

                return _responseFactory.CreateResponse(StatusCode.Fail, messageResponse);
            }
        }

        public async Task<IResponse> StopMessagesReceivingAsync(PHPScriptExecutor runningScript)
        {
            string messageResponse = string.Empty;
            try
            {
                _logger.LogInformation("Stopping message receiving in [{runningScriptName}] started", runningScript.ExecutableScript.Name);
                if (!runningScript.IsRunning)
                {
                    _logger.LogError("[{runningScriptName}] was not running", runningScript.ExecutableScript.Name);
                    messageResponse = $"{runningScript.ExecutableScript.Name} was not running top receiving messages is not available";

                    return _responseFactory.CreateResponse(StatusCode.Fail, messageResponse);
                }

                if (!runningScript.IsMessagesReceiving)
                {
                    _logger.LogError("[{RunningScriptName}] messages receiving wasn't started", runningScript.ExecutableScript.Name);
                    messageResponse = $"{runningScript.ExecutableScript.Name} script messages receiving wasn't started";

                    return _responseFactory.CreateResponse(StatusCode.Fail, messageResponse);
                }

                await runningScript.StopMessagesReceivingAsync().ConfigureAwait(false);
                _logger.LogInformation("Stopping message receiving successfully completed");
                messageResponse = "Message receiving was successfully stopped";

                return _responseFactory.CreateResponse(StatusCode.Success, messageResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{ExcetptionType} was catched", typeof(Exception));
                messageResponse = "Something go wrong";

                return _responseFactory.CreateResponse(StatusCode.Fail, messageResponse);
            }
        }
    }
}
