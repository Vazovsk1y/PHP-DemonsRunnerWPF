using DemonsRunner.BuisnessLayer.Services.Interfaces;
using DemonsRunner.Commands;
using DemonsRunner.Domain.Enums;
using DemonsRunner.Domain.Models;
using DemonsRunner.Domain.Responses.Intefaces;
using DemonsRunner.Infrastructure.Extensions;
using DemonsRunner.Infrastructure.Messages;
using DemonsRunner.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace DemonsRunner.ViewModels
{
    internal class WorkSpaceViewModel : BaseViewModel, IDisposable
    {
        #region --Fields--

        private bool? _isStartButtonEnable = null;
        private bool? _isStopButtonEnable = null;
        private bool _showExecutingWindow = false;
        private ObservableCollection<PHPScript> _configuredScripts;
        private readonly ObservableCollection<IScriptExecutorViewModel> _runningScriptsViewModels = new();
        private readonly FilesPanelViewModel _filesPanelViewModel;
        private readonly IScriptConfigureService _configureSctiptsService;
        private readonly IScriptExecutorService _executorScriptsService;
        private readonly IScriptExecutorViewModelFactory _scriptExecutorViewModelFactory;
        private readonly IDisposable _subscription;
        private readonly IDataBus _dataBus;

        #endregion

        #region --Properties--

        public bool? IsStartButtonEnable
        {
            get => _isStartButtonEnable is bool condition
                    ? condition
                    : ConfiguredScripts is ICollection<PHPScript> { Count: > 0 } &&
                      RunningScriptsViewModels is ICollection<IScriptExecutorViewModel> { Count: 0 };
            set 
            {
                if (Set(ref _isStartButtonEnable, value))
                {
                    OnPropertyChanged(nameof(StartScriptsCommand));
                }
            }
        }

        public bool? IsStopButtonEnable
        {
            get => _isStopButtonEnable is bool condition
                    ? condition
                    : RunningScriptsViewModels is ICollection<IScriptExecutorViewModel> { Count: > 0 };
            set
            {
                if (Set(ref _isStopButtonEnable, value))
                {
                    OnPropertyChanged(nameof(StopScriptsCommand));
                }
            }
        }

        public ObservableCollection<IScriptExecutorViewModel> RunningScriptsViewModels => _runningScriptsViewModels;

        public bool ShowExecutingWindow
        {
            get => _showExecutingWindow;
            set => Set(ref _showExecutingWindow, value);
        }

        public ObservableCollection<PHPScript> ConfiguredScripts
        {
            get => _configuredScripts;
            set => Set(ref _configuredScripts, value);
        }

        #endregion

        #region --Constructors--

        public WorkSpaceViewModel() 
        {

        }

        public WorkSpaceViewModel(
            FilesPanelViewModel filesPanelViewModel,
            IScriptConfigureService configureSctiptsService,
            IScriptExecutorService executorScriptsService, 
            IScriptExecutorViewModelFactory scriptExecutorViewModelFactory,
            IDataBus dataBus)
        {
            _filesPanelViewModel = filesPanelViewModel;
            _configureSctiptsService = configureSctiptsService;
            _executorScriptsService = executorScriptsService;
            _scriptExecutorViewModelFactory = scriptExecutorViewModelFactory;
            _dataBus = dataBus;
            _subscription = _dataBus.RegisterHandler<ScriptExitedMessage>(OnScriptExited);
        }

        #endregion

        #region --Commands--

        public ICommand ConfigureScriptsCommand => new RelayCommand(
            OnConfigureScriptsExecute,
            (arg) => _filesPanelViewModel.Demons.Count > 0);

        private async void OnConfigureScriptsExecute(object obj)
        {
            var response = await _configureSctiptsService.ConfigureScripts(_filesPanelViewModel.Demons).ConfigureAwait(false);
            if (response.OperationStatus == StatusCode.Success)
            {
                await App.Current.Dispatcher.InvokeAsync(() =>
                {
                    ConfiguredScripts = new ObservableCollection<PHPScript>(response.Data!);
                });
            }
            _dataBus.Send(response.Description);
        }

        public ICommand ClearConfigureScripts => new RelayCommand(
            (arg) => ConfiguredScripts.Clear(),
            (arg) => ConfiguredScripts is ICollection<PHPScript> { Count: > 0 });

        public ICommand StartScriptsCommand => new RelayCommand(
            OnStartScriptsExecute,
            (arg) => (bool)IsStartButtonEnable!);

        private async void OnStartScriptsExecute(object obj)
        {
            IsStartButtonEnable = false;
            var (responses, executorsViewModels) = await GetStartingResult().ConfigureAwait(false);
            var failedResponses = responses.Where(r => r.OperationStatus is StatusCode.Fail).ToList();

            await App.Current.Dispatcher.InvokeAsync(() =>
            {
                RunningScriptsViewModels.AddRange(executorsViewModels);
            });

            if (failedResponses.Count is 0)
            {
                _dataBus.Send($"{executorsViewModels.ToList().Count} scripts were successfully started");
            }
            else
            {
                _dataBus.SendDescriptions(failedResponses);
            }
            IsStopButtonEnable = true;
        }

        public ICommand StopScriptsCommand => new RelayCommand(
            OnStopScriptsExecute,
            (arg) => (bool)IsStopButtonEnable!);

        private async void OnStopScriptsExecute(object obj)
        {
            IsStopButtonEnable = false;
            var (responses, successfullyStoppedViewModels) = await GetStoppingResult().ConfigureAwait(false);
            var failedResponses = responses.Where(r => r.OperationStatus is StatusCode.Fail).ToList();

            await App.Current.Dispatcher.InvokeAsync(() =>
            {
                RunningScriptsViewModels.RemoveAll(successfullyStoppedViewModels);
            });

            if (failedResponses.Count is 0)
            {
                _dataBus.Send($"All scripts were succsessfully stopped");
            }
            else
            {
                _dataBus.SendDescriptions(failedResponses);
            }
            IsStartButtonEnable = true;
        }

        #endregion

        #region --Methods--

        public void Dispose()
        {
            _subscription.Dispose();
            if (RunningScriptsViewModels.Count > 0)
            {
                foreach (var scriptViewModel in RunningScriptsViewModels)
                {
                    scriptViewModel.Dispose();
                }
                RunningScriptsViewModels.Clear();
            }
        }

        private async void OnScriptExited(ScriptExitedMessage message)
        {
            if (!RunningScriptsViewModels.Contains(message.Sender))
            {
                return;
            }

            switch (message.ExitType)
            {
                case ExitType.ByTaskManager:
                    {
                        await App.Current.Dispatcher.InvokeAsync(() =>
                        {
                            RunningScriptsViewModels.Remove(message.Sender);
                        });
                        _dataBus.Send($"{message.Sender.ScriptExecutor.ExecutableScript.Name} was killed in task manager");
                        message.Sender.Dispose();
                        break;
                    }
                case ExitType.ByAppInfrastructure:
                    {
                        var stoppingMessageReceivingResponse = await _executorScriptsService.StopMessagesReceivingAsync(message.Sender.ScriptExecutor);
                        var stoppingResponse = await _executorScriptsService.StopAsync(message.Sender.ScriptExecutor);
                        await App.Current.Dispatcher.InvokeAsync(() =>
                        {
                            RunningScriptsViewModels.Remove(message.Sender);
                        });
                        _dataBus.Send(stoppingMessageReceivingResponse.Description);
                        _dataBus.Send(stoppingResponse.Description);
                        message.Sender.Dispose();
                        break;
                    }
            }
            IsStopButtonEnable = null;
            IsStartButtonEnable = null;
        }

        private async Task<(IEnumerable<IResponse> responses, IEnumerable<IScriptExecutorViewModel> successfullyStartedViewModels)> GetStartingResult()
        {
            var successfullyStartedViewModels = new List<IScriptExecutorViewModel>();
            var responses = new List<IResponse>();

            await Task.Run(async () =>
            {
                foreach (var script in ConfiguredScripts.ToList())
                {
                    var startingResponse = await _executorScriptsService.StartAsync(script, ShowExecutingWindow).ConfigureAwait(false);
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
            });

            return (responses, successfullyStartedViewModels);
        }

        private async Task<(IEnumerable<IResponse> responses, IEnumerable<IScriptExecutorViewModel> successfullyStoppedViewModels)> GetStoppingResult()
        {
            var responses = new List<IResponse>();
            var successfullyStoppedViewModels = new List<IScriptExecutorViewModel>();

            await Task.Run(async () =>
            {
                foreach (var scriptExecutorViewModel in RunningScriptsViewModels.ToList())
                {
                    var messageReceivingResponse = await _executorScriptsService.StopMessagesReceivingAsync(scriptExecutorViewModel.ScriptExecutor).ConfigureAwait(false);
                    responses.Add(messageReceivingResponse);
                    var stoppingResponse = await _executorScriptsService.StopAsync(scriptExecutorViewModel.ScriptExecutor).ConfigureAwait(false);
                    responses.Add(stoppingResponse);

                    if (messageReceivingResponse.OperationStatus is StatusCode.Success &&
                        stoppingResponse.OperationStatus is StatusCode.Success)
                    {
                        successfullyStoppedViewModels.Add(scriptExecutorViewModel);
                    }
                    scriptExecutorViewModel.Dispose();
                }
            });

            return (responses, successfullyStoppedViewModels);
        }

        #endregion
    }
}
