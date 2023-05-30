using DemonsRunner.BuisnessLayer.Services.Interfaces;
using DemonsRunner.Domain.Models;
using DemonsRunner.Infrastructure.Messages;
using DemonsRunner.ViewModels.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Concurrent;

namespace DemonsRunner.ViewModels
{
    internal class PHPScriptExecutorViewModelFactory : IScriptExecutorViewModelFactory, IDisposable
    {
        private readonly IDataBus _dataBus;
        private readonly IDisposable _subscription;
        private readonly IServiceProvider _serviceProvider;
        private readonly IScriptExecutorService _executorScriptsService;
        private readonly ConcurrentDictionary<IScriptExecutorViewModel, IServiceScope> _viewModelsLifetimeControlStorage = new();

        public PHPScriptExecutorViewModelFactory(
            IDataBus dataBus,
            IServiceProvider serviceProvider,
            IScriptExecutorService executorScriptsService)
        {
            _dataBus = dataBus;
            _serviceProvider = serviceProvider;
            _executorScriptsService = executorScriptsService;
            _subscription = _dataBus.RegisterHandler<ScriptExitedMessage>(OnScriptExited);
        }

        public void Dispose() => _subscription.Dispose();

        public IScriptExecutorViewModel CreateViewModel(PHPScriptExecutor scriptExecutor)
        {
            var scope = _serviceProvider.CreateScope();
            var viewModel = new PHPScriptExecutorViewModel(scriptExecutor, _dataBus);
            _viewModelsLifetimeControlStorage[viewModel] = scope;

            return viewModel;
        }

        private async void OnScriptExited(ScriptExitedMessage message)
        {
            if (!_viewModelsLifetimeControlStorage.TryRemove(message.Sender, out var scope))
            {
                return;
            }

            switch (message.ExitType)
            {
                case ExitType.ByTaskManager:
                    {
                        _dataBus.Send($"{message.Sender.ScriptExecutor.ExecutableScript.Name} was killed in task manager.");
                        message.Sender.Dispose();
                        scope.Dispose();
                        break;
                    }
                case ExitType.ByAppInfrastructure:
                    {
                        var stoppingMessageReceivingResponse = await _executorScriptsService.StopMessagesReceivingAsync(message.Sender.ScriptExecutor);
                        var stoppingResponse = await _executorScriptsService.StopAsync(message.Sender.ScriptExecutor);
                        _dataBus.Send(stoppingMessageReceivingResponse.Description);
                        _dataBus.Send(stoppingResponse.Description);
                        message.Sender.Dispose();
                        scope.Dispose();
                        break;
                    }
            }
        }
    }
            
}
