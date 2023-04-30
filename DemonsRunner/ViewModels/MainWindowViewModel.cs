using DemonsRunner.Commands;
using DemonsRunner.Domain.Enums;
using DemonsRunner.Domain.Interfaces;
using DemonsRunner.Domain.Models;
using DemonsRunner.Domain.Services;
using DemonsRunner.Infrastructure.Extensions;
using DemonsRunner.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace DemonsRunner.ViewModels
{
    internal class MainWindowViewModel : BaseViewModel
    {
        #region --Fields--

        private string _windowTitle = "Main";
        private bool _showExecutingWindow = false;
        private PHPDemon _selectedDemon;
        private ObservableCollection<PHPScript> _configuredScripts;
        private readonly ObservableCollection<PHPScriptExecutorViewModel> _runningScriptsViewModels = new();
        private readonly ObservableCollection<PHPDemon> _demons = new();
        private readonly IFileDialogService _dialogService;
        private readonly IScriptConfigureService _configureSctiptsService;
        private readonly IScriptExecutorService _executorScriptsService;
        private readonly IFileService _fileService;

        #endregion

        #region --Properties--

        public ObservableCollection<PHPScriptExecutorViewModel> RunningScriptsViewModels => _runningScriptsViewModels;

        public ObservableCollection<PHPDemon> Demons => _demons;

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

        public PHPDemon SelectedDemon
        {
            get => _selectedDemon;
            set => Set(ref _selectedDemon, value);
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
            IFileDialogService dialogService, 
            IScriptConfigureService configureSctiptsService, 
            IScriptExecutorService executorScriptsService, 
            IFileService fileService)
        {
            _dialogService = dialogService;
            _configureSctiptsService = configureSctiptsService;
            _executorScriptsService = executorScriptsService;
            _fileService = fileService;
            var response = _fileService.GetSaved();
            if (response.OperationStatus == StatusCode.Success)
            {
                Demons.AddRange(response.Data);
            }
            Demons.CollectionChanged += (s, e) => _fileService.Save(Demons);
        }

        #endregion

        #region --Commands--

        public ICommand AddFileToListCommand => new RelayCommand(OnAddingFileExecute);

        private async void OnAddingFileExecute(object obj)
        {
            var response = _dialogService.StartDialog();
            if (response.OperationStatus == StatusCode.Success)
            {
                await App.Current.Dispatcher.InvokeAsync(() =>
                {
                    Demons.AddIfNotExist(response.Data);
                });
            }
        }

        public ICommand DeleteFileFromListCommand => new RelayCommand(OnDeletingFileExecute, 
            (arg) => Demons.Count > 0 && SelectedDemon is not null);

        private void OnDeletingFileExecute(object obj)
        {
           if (Demons.Contains(SelectedDemon))
           {
                Demons.Remove(SelectedDemon);
                SelectedDemon = null;
           }
        }

        public ICommand ConfigureScriptsCommand => new RelayCommand(OnConfigureScriptsExecute, 
            (arg) => Demons.Count > 0);

        private async void OnConfigureScriptsExecute(object obj)
        {
            var response = _configureSctiptsService.ConfigureScripts(Demons);
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
            foreach (var script in ConfiguredScripts)
            {
                var response = await _executorScriptsService.StartAsync(script, ShowExecutingWindow).ConfigureAwait(false);
                if (response.OperationStatus == StatusCode.Success)
                {
                    await App.Current.Dispatcher.InvokeAsync(() =>
                    {
                        var executorViewModel = new PHPScriptExecutorViewModel(response.Data);
                        executorViewModel.ScriptExited += OnScriptExited;
                        RunningScriptsViewModels.Add(executorViewModel);
                    });
                }
            }
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
                    await App.Current.Dispatcher.InvokeAsync(() =>
                    {
                        scriptExecutorViewModel.ScriptExited -= OnScriptExited;
                        scriptExecutorViewModel.Dispose();
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
