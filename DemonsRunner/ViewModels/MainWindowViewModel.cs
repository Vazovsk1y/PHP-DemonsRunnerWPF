using DemonsRunner.Commands;
using DemonsRunner.Domain.Enums;
using DemonsRunner.Domain.Models;
using DemonsRunner.Domain.Services;
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
    internal class MainWindowViewModel : BaseViewModel
    {
        #region --Fields--

        private string _windowTitle = "Main";
        private bool _showExecutingWindow = false;
        private ObservableCollection<PHPScript> _configuredScripts;
        private readonly ObservableCollection<PHPScriptExecutorViewModel> _runningScriptsViewModels = new();
        private readonly IScriptConfigureService _configureSctiptsService;
        private readonly IScriptExecutorService _executorScriptsService;

        #endregion

        #region --Properties--

        public ObservableCollection<PHPScriptExecutorViewModel> RunningScriptsViewModels => _runningScriptsViewModels;

        public FilesPanelViewModel FilesPanelViewModel { get; }

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

        public string WindowTitle
        {
            get => _windowTitle;
            set => Set(ref _windowTitle, value);
        }

        #endregion

        #region --Constructors--

        public MainWindowViewModel()
        {

        }

        public MainWindowViewModel(
            FilesPanelViewModel filesPanelViewModel,
            IScriptConfigureService configureSctiptsService, 
            IScriptExecutorService executorScriptsService) 
        {
            FilesPanelViewModel = filesPanelViewModel;
            _configureSctiptsService = configureSctiptsService;
            _executorScriptsService = executorScriptsService;
        }

        #endregion

        #region --Commands--

        public ICommand ConfigureScriptsCommand => new RelayCommand(OnConfigureScriptsExecute, 
            (arg) => FilesPanelViewModel.Demons.Count > 0);

        private async void OnConfigureScriptsExecute(object obj)
        {
            var response = _configureSctiptsService.ConfigureScripts(FilesPanelViewModel.Demons);
            if (response.OperationStatus == StatusCode.Success)
            {
                await App.Current.Dispatcher.InvokeAsync(() =>
                {
                    ConfiguredScripts = new ObservableCollection<PHPScript>(response.Data);
                });
            }
        }

        public ICommand ClearConfigureScripts => new RelayCommand(OnClearConfiguredScriptsExecute, 
            (arg) => ConfiguredScripts is ICollection<PHPScript> { Count: > 0 });

        private void OnClearConfiguredScriptsExecute(object obj) => ConfiguredScripts.Clear();

        public ICommand StartScriptsCommand => new RelayCommand(OnStartScriptsExecute, 
            (arg) => ConfiguredScripts is ICollection<PHPScript> { Count: > 0 } &&
            RunningScriptsViewModels is not ICollection<PHPScriptExecutorViewModel> { Count: > 0 });

        private async void OnStartScriptsExecute(object obj)
        {
            var viewModels = new List<PHPScriptExecutorViewModel>();
            await Task.Run(async () =>
            {
                foreach (var script in ConfiguredScripts)
                {
                    var response = await _executorScriptsService.StartExecutingAsync(script, ShowExecutingWindow).ConfigureAwait(false);
                    if (response.OperationStatus == StatusCode.Success)
                    {
                        var executorViewModel = new PHPScriptExecutorViewModel(response.Data);
                        executorViewModel.ScriptExited += OnScriptExited;
                        viewModels.Add(executorViewModel);
                    }
                }
            });

            await App.Current.Dispatcher.InvokeAsync(() =>
            {
                RunningScriptsViewModels.AddRange(viewModels);
            });
        }

        public ICommand StopScriptsCommand => new RelayCommand(OnStopScriptsExecute,
            (arg) => RunningScriptsViewModels is ICollection<PHPScriptExecutorViewModel> { Count: > 0 });

        private async void OnStopScriptsExecute(object obj)
        {
            foreach (var scriptExecutorViewModel in RunningScriptsViewModels.ToList())
            {
                var response = await _executorScriptsService.StopAsync(scriptExecutorViewModel.ScriptExecutor).ConfigureAwait(false);
                if (response.OperationStatus == StatusCode.Success)
                {
                    scriptExecutorViewModel.ScriptExited -= OnScriptExited;
                    scriptExecutorViewModel.Dispose();
                    await App.Current.Dispatcher.InvokeAsync(() =>
                    {
                        RunningScriptsViewModels.Remove(scriptExecutorViewModel);
                    });
                }
            }
        }

        #endregion

        #region --Methods--

        private async void OnScriptExited(object? sender, EventArgs e)
        {
            if (sender is PHPScriptExecutorViewModel scriptExecutorViewModel)
            {
                await App.Current.Dispatcher.InvokeAsync(() =>
                {
                    scriptExecutorViewModel.ScriptExited -= OnScriptExited;
                    scriptExecutorViewModel.Dispose();
                    RunningScriptsViewModels.Remove(scriptExecutorViewModel);
                });
            }
        }

        #endregion
    }
}
