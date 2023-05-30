using DemonsRunner.BuisnessLayer.Services.Interfaces;
using DemonsRunner.Domain.Enums;
using DemonsRunner.Domain.Models;
using DemonsRunner.Domain.Responses.Intefaces;
using DemonsRunner.Infrastructure.Managers.Interfaces;
using DemonsRunner.ViewModels.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DemonsRunner.Infrastructure.Managers
{
    internal class ServiceManager : IServiceManager
    {
        private readonly IScriptExecutorService _executorScriptsService;
        private readonly IScriptExecutorViewModelFactory _scriptExecutorViewModelFactory;

        public ServiceManager(
            IScriptExecutorService executorScriptsService, 
            IScriptExecutorViewModelFactory scriptExecutorViewModelFactory)
        {
            _executorScriptsService = executorScriptsService;
            _scriptExecutorViewModelFactory = scriptExecutorViewModelFactory;
        }

        public async Task<(IEnumerable<IResponse> responses, IEnumerable<IScriptExecutorViewModel> successfullyStartedViewModels)> 
            GetStartingResultAsync(IEnumerable<PHPScript> configuredScripts)
        {
            var successfullyStartedViewModels = new List<IScriptExecutorViewModel>();
            var responses = new List<IResponse>();

            await Task.Run(async () =>
            {
                foreach (var script in configuredScripts.ToList())
                {
                    var startingResponse = await _executorScriptsService.StartAsync(script).ConfigureAwait(false);
                    responses.Add(startingResponse);
                    if (startingResponse.OperationStatus is StatusCode.Success)
                    {
                        var executorViewModel = _scriptExecutorViewModelFactory.CreateViewModel(startingResponse.Data!);
                        successfullyStartedViewModels.Add(executorViewModel);

                        var messageReceivingResponse = await _executorScriptsService.StartMessagesReceivingAsync(startingResponse.Data!).ConfigureAwait(false);
                        responses.Add(messageReceivingResponse);

                        var executingCommandResponse = await _executorScriptsService.ExecuteCommandAsync(startingResponse.Data!);
                        responses.Add(executingCommandResponse);
                    }
                }
            }).ConfigureAwait(false);

            return (responses, successfullyStartedViewModels);
        }

        public async Task<(IEnumerable<IResponse> responses, IEnumerable<IScriptExecutorViewModel> successfullyStoppedViewModels)> 
            GetStoppingResultAsync(IEnumerable<IScriptExecutorViewModel> runningViewModels)
        {
            var responses = new List<IResponse>();
            var successfullyStoppedViewModels = new List<IScriptExecutorViewModel>();

            await Task.Run(async () =>
            {
                foreach (var scriptExecutorViewModel in runningViewModels.ToList())
                {
                    var messageReceivingResponse = await _executorScriptsService.StopMessagesReceivingAsync(scriptExecutorViewModel.ScriptExecutor).ConfigureAwait(false);
                    responses.Add(messageReceivingResponse);
                    var stoppingResponse = await _executorScriptsService.StopAsync(scriptExecutorViewModel.ScriptExecutor).ConfigureAwait(false);
                    responses.Add(stoppingResponse);

                    if (stoppingResponse.OperationStatus is StatusCode.Success)
                    {
                        successfullyStoppedViewModels.Add(scriptExecutorViewModel);
                    }
                    scriptExecutorViewModel.Dispose();
                }
            }).ConfigureAwait(false);

            return (responses, successfullyStoppedViewModels);
        }
    }
}
