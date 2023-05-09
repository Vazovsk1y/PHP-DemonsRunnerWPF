using DemonsRunner.Domain.Responses;
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

        public ScriptExecutorService(IFileService fileService, ILogger<ScriptExecutorService> logger) 
        {
            _fileService = fileService;
            _logger = logger;
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
                    return new DataResponse<PHPScriptExecutor>
                    {
                        Description = $"Executable {response.Description}",
                        OperationStatus = StatusCode.Fail,
                    };
                }

                var executor = new PHPScriptExecutor(script, showExecutingWindow);
                if (await executor.StartAsync().ConfigureAwait(false))
                {
                    _logger.LogInformation("Cmd was started successfully");
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

        public async Task<IResponse> StartMessagesReceivingAsync(PHPScriptExecutor runningScript)
        {
            try
            {
                _logger.LogInformation("Starting messages reception of [{runningScriptName}] has started", runningScript.ExecutableScript.Name);
                if (!runningScript.IsRunning)
                {
                    _logger.LogError("[{RunningScriptName}] was not running", runningScript.ExecutableScript.Name);
                    return new Response
                    {
                        Description = "Script was not running",
                        OperationStatus = StatusCode.Fail,
                    };
                }

                await runningScript.StartMessagesReceivingAsync().ConfigureAwait(false);
                _logger.LogInformation("Starting messages receiving was started successfully");
                return new Response
                {
                    Description = "Messages receiving was started successfully",
                    OperationStatus = StatusCode.Success,
                };
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "{ExcetptionType} was catched", typeof(Exception));
                return new Response
                {
                    Description = "Something go wrong",
                    OperationStatus = StatusCode.Fail
                };
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
                    return new Response
                    {
                        Description = "Script was not running",
                        OperationStatus = StatusCode.Fail,
                    };
                }

                await runningScript.ExecuteCommandAsync().ConfigureAwait(false);
                _logger.LogInformation("Executing command was completed successfully");
                return new Response
                {
                    Description = "Command was executed successfully",
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

        public async Task<IResponse> StopAsync(PHPScriptExecutor executingScript)
        {
            try
            {
                _logger.LogInformation("Stopping script [{executingScriptName}] executing started.", executingScript.ExecutableScript.Name);
                if (!executingScript.IsRunning)
                {
                    _logger.LogInformation("[{executingScriptName}] was not running", executingScript.ExecutableScript.Name);
                    return new Response
                    {
                        Description = $"{executingScript.ExecutableScript.Name} is not running!",
                        OperationStatus = StatusCode.Fail
                    };
                }

                await executingScript.StopAsync().ConfigureAwait(false);
                _logger.LogInformation("Cmd was killed successfully");

                return new Response
                {
                    Description = "Script was killed successfully!",
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

        public async Task<IResponse> StopMessagesReceivingAsync(PHPScriptExecutor runningScript)
        {
            try
            {
                _logger.LogInformation("Stopping message receiving in [{runningScriptName}] started", runningScript.ExecutableScript.Name);
                if (!runningScript.IsRunning)
                {
                    _logger.LogError("[{RunningScript}] was not running", runningScript.ExecutableScript.Name);
                    return new Response
                    {
                        Description = "Script was not running",
                        OperationStatus = StatusCode.Fail,
                    };
                }

                await runningScript.StopMessagesReceivingAsync().ConfigureAwait(false);
                _logger.LogInformation("Stopping message receiving successfully completed");
                return new Response
                {
                    Description = "Message receiving was successfully stopped",
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
