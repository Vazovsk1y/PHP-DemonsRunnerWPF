using DemonsRunner.BuisnessLayer.Services.Interfaces;
using DemonsRunner.Commands;
using DemonsRunner.Domain.Enums;
using DemonsRunner.Domain.Models;
using DemonsRunner.Infrastructure.Extensions;
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
        private readonly ObservableCollection<PHPScriptExecutorViewModel> _runningScriptsViewModels = new();
        private readonly FilesPanelViewModel _filesPanelViewModel;
        private readonly IScriptConfigureService _configureSctiptsService;
        private readonly IScriptExecutorService _executorScriptsService;

        #endregion

        #region --Properties--

        public bool IsButtonStartScriptsPressed
        {
            get => _isButtonStartScriptsPressed;
            set => Set(ref _isButtonStartScriptsPressed, value);
        }

        public ObservableCollection<PHPScriptExecutorViewModel> RunningScriptsViewModels => _runningScriptsViewModels;

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
            IScriptExecutorService executorScriptsService)
        {
            _filesPanelViewModel = filesPanelViewModel;
            _configureSctiptsService = configureSctiptsService;
            _executorScriptsService = executorScriptsService;
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
            RunningScriptsViewModels is ICollection<PHPScriptExecutorViewModel> { Count: 0 } && 
            !IsButtonStartScriptsPressed);

        private async void OnStartScriptsExecute(object obj)
        {
            IsButtonStartScriptsPressed = true;
            var viewModels = new List<PHPScriptExecutorViewModel>();
            await Task.Run(async () =>
            {
                foreach (var script in ConfiguredScripts.ToList())
                {
                    var response = await _executorScriptsService.StartAsync(script, ShowExecutingWindow).ConfigureAwait(false);
                    if (response.OperationStatus == StatusCode.Success)
                    {
                        var executorViewModel = new PHPScriptExecutorViewModel(response.Data);
                        await _executorScriptsService.StartMessagesReceivingAsync(executorViewModel.ScriptExecutor);
                        await _executorScriptsService.ExecuteCommandAsync(executorViewModel.ScriptExecutor);
                        executorViewModel.ScriptExited += OnScriptExited;
                        viewModels.Add(executorViewModel);
                    }
                }
            }).ConfigureAwait(false);

            await App.Current.Dispatcher.InvokeAsync(() =>
            {
                RunningScriptsViewModels.AddRange(viewModels);
            });

            // update button unavailable state to prevent clicking when scripts are executed.
            OnPropertyChanged(nameof(StopScriptsCommand));
        }

        public ICommand StopScriptsCommand => new RelayCommand(
            OnStopScriptsExecute,
            (arg) => RunningScriptsViewModels is ICollection<PHPScriptExecutorViewModel> { Count: > 0 });

        private async void OnStopScriptsExecute(object obj)
        {
            foreach (var scriptExecutorViewModel in RunningScriptsViewModels.ToList())
            {
                var messageReceivingResponse = await _executorScriptsService.StopMessagesReceivingAsync(scriptExecutorViewModel.ScriptExecutor).ConfigureAwait(false);
                var stoppingResponse = await _executorScriptsService.StopAsync(scriptExecutorViewModel.ScriptExecutor).ConfigureAwait(false);
                if (stoppingResponse.OperationStatus == StatusCode.Success && messageReceivingResponse.OperationStatus == StatusCode.Success)
                {
                    scriptExecutorViewModel.ScriptExited -= OnScriptExited;
                    scriptExecutorViewModel.Dispose();
                    await App.Current.Dispatcher.InvokeAsync(() =>
                    {
                        RunningScriptsViewModels.Remove(scriptExecutorViewModel);
                    });
                }
            }
            IsButtonStartScriptsPressed = false;
        }

        #endregion

        #region --Methods--

        private async void OnScriptExited(object? sender, EventArgs e)
        {
            if (sender is PHPScriptExecutorViewModel scriptExecutorViewModel)
            {
                scriptExecutorViewModel.ScriptExited -= OnScriptExited;
                scriptExecutorViewModel.Dispose();
                await App.Current.Dispatcher.InvokeAsync(() =>
                {
                    RunningScriptsViewModels.Remove(scriptExecutorViewModel);
                });
            }
        }

        #endregion
    }
}
