using DemonsRunner.BuisnessLayer.Services.Interfaces;
using DemonsRunner.Commands;
using DemonsRunner.Domain.Enums;
using DemonsRunner.Domain.Models;
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
    internal class WorkSpaceViewModel : BaseViewModel
    {
        #region --Fields--

        private bool _isButtonStartScriptsPressed = false;
        private bool _showExecutingWindow = false;
        private ObservableCollection<PHPScript> _configuredScripts;
        private readonly ObservableCollection<IScriptExecutorViewModel> _runningScriptsViewModels = new();
        private readonly FilesPanelViewModel _filesPanelViewModel;
        private readonly IScriptConfigureService _configureSctiptsService;
        private readonly IScriptExecutorService _executorScriptsService;
        private readonly IScriptExecutorViewModelFactory _scriptExecutorViewModelFactory;
        private readonly IDataBus _dataBus;
        private readonly IDisposable _subscription;

        #endregion

        #region --Properties--

        public bool IsButtonStartScriptsPressed
        {
            get => _isButtonStartScriptsPressed;
            set => Set(ref _isButtonStartScriptsPressed, value);
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
                    ConfiguredScripts = new ObservableCollection<PHPScript>(response.Data);
                });
            }
        }

        public ICommand ClearConfigureScripts => new RelayCommand(
            (arg) => ConfiguredScripts.Clear(),
            (arg) => ConfiguredScripts is ICollection<PHPScript> { Count: > 0 });

        public ICommand StartScriptsCommand => new RelayCommand(
            OnStartScriptsExecute,
            (arg) => 
            ConfiguredScripts is ICollection<PHPScript> { Count: > 0 } &&
            RunningScriptsViewModels is ICollection<IScriptExecutorViewModel> { Count: 0 } && 
            !IsButtonStartScriptsPressed);

        private async void OnStartScriptsExecute(object obj)
        {
            IsButtonStartScriptsPressed = true;
            var viewModels = new List<IScriptExecutorViewModel>();
            await Task.Run(async () =>
            {
                foreach (var script in ConfiguredScripts.ToList())
                {
                    var response = await _executorScriptsService.StartAsync(script, ShowExecutingWindow).ConfigureAwait(false);
                    if (response.OperationStatus == StatusCode.Success)
                    {
                        var executorViewModel = _scriptExecutorViewModelFactory.CreateViewModel(response.Data);
                        await _executorScriptsService.StartMessagesReceivingAsync(executorViewModel.ScriptExecutor);
                        await _executorScriptsService.ExecuteCommandAsync(executorViewModel.ScriptExecutor);
                        viewModels.Add(executorViewModel);
                    }
                }
            }).ConfigureAwait(false);

            await App.Current.Dispatcher.InvokeAsync(() =>
            {
                RunningScriptsViewModels.AddRange(viewModels);
            });

            // update button unavailable state to prevent clicking when scripts are not already executed.
            OnPropertyChanged(nameof(StopScriptsCommand));
        }

        public ICommand StopScriptsCommand => new RelayCommand(
            OnStopScriptsExecute,
            (arg) => RunningScriptsViewModels is ICollection<IScriptExecutorViewModel> { Count: > 0 });

        private async void OnStopScriptsExecute(object obj)
        {
            foreach (var scriptExecutorViewModel in RunningScriptsViewModels.ToList())
            {
                var messageReceivingResponse = await _executorScriptsService.StopMessagesReceivingAsync(scriptExecutorViewModel.ScriptExecutor).ConfigureAwait(false);
                var stoppingResponse = await _executorScriptsService.StopAsync(scriptExecutorViewModel.ScriptExecutor).ConfigureAwait(false);
                if (stoppingResponse.OperationStatus == StatusCode.Success && messageReceivingResponse.OperationStatus == StatusCode.Success)
                {
                    await App.Current.Dispatcher.InvokeAsync(() =>
                    {
                        RunningScriptsViewModels.Remove(scriptExecutorViewModel);
                    });
                    scriptExecutorViewModel.Dispose();
                }
            }
            IsButtonStartScriptsPressed = false;
        }

        #endregion

        #region --Methods--

        private async void OnScriptExited(ScriptExitedMessage message)
        {
            if (message.Sender is IScriptExecutorViewModel scriptExecutorViewModel && RunningScriptsViewModels.Contains(scriptExecutorViewModel))
            {
                await App.Current.Dispatcher.InvokeAsync(() =>
                {
                    RunningScriptsViewModels.Remove(scriptExecutorViewModel);
                });
                scriptExecutorViewModel.Dispose();
            }
        }

        #endregion
    }
}
