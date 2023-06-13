using DaemonsRunner.BuisnessLayer.Services.Interfaces;
using DaemonsRunner.Domain.Models;
using DaemonsRunner.Infrastructure.Extensions;
using DaemonsRunner.Infrastructure.Managers.Interfaces;
using DaemonsRunner.Infrastructure.Messages;
using DaemonsRunner.ViewModels.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Concurrent;
using System.Linq;

namespace DaemonsRunner.ViewModels
{
    internal class PHPScriptExecutorViewModelFactory : IScriptExecutorViewModelFactory, IDisposable
    {
        // The class responsibility is to control the life cycle of the view models that are created by the factory.

        private readonly IDataBus _dataBus;
        private readonly IDisposable _subscription;
        private readonly IServiceProvider _serviceProvider;
        private readonly ConcurrentDictionary<IScriptExecutorViewModel, IServiceScope> _viewModelsLifetimeControlStorage = new();

        public PHPScriptExecutorViewModelFactory(
            IDataBus dataBus,
            IServiceProvider serviceProvider)
        {
            _dataBus = dataBus;
            _serviceProvider = serviceProvider;
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
            if (!_viewModelsLifetimeControlStorage.TryRemove(message.Sender, out var viewModelScope))
            {
                return;
            }

            switch (message.ExitType)
            {
                case ExitType.ByTaskManager:
                    {
                        _dataBus.Send($"{message.Sender.ScriptExecutor.ExecutableScript.Name} was killed in task manager.");
                        message.Sender.Dispose();
                        viewModelScope.Dispose();

                        break;
                    }
                case ExitType.ByAppInfrastructure:
                    {
                        using var scope = _serviceProvider.CreateScope();
                        var serviceManager = scope.ServiceProvider.GetRequiredService<IServiceManager>();

                        var stoppingResult = await serviceManager.GetStoppingResultAsync(message.Sender);
                        _dataBus.SendAll(stoppingResult.Select(r => r.Description));
                        message.Sender.Dispose();
                        viewModelScope.Dispose();

                        break;
                    }
            }
        }
    }

}
