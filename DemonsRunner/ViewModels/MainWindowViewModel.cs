using DemonsRunner.Commands;
using DemonsRunner.Domain.Enums;
using DemonsRunner.Domain.Interfaces;
using DemonsRunner.Domain.Models;
using DemonsRunner.Domain.Services;
using DemonsRunner.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
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
        private ObservableCollection<PHPScriptExecutor> _runningScripts;
        private readonly ObservableCollection<PHPScriptOutput> _scriptMessages = new();
        private readonly ObservableCollection<PHPDemon> _demons = new();
        private readonly IFileDialogService _dialogService;
        private readonly IScriptConfigureService _configureSctiptsService;
        private readonly IScriptExecutorService _executorScriptsService;
        private readonly IFileService _fileService;

        #endregion

        #region --Properties--

        public ObservableCollection<PHPScriptOutput> ScriptsOutputs => _scriptMessages;

        public bool ShowExecutingWindow
        {
            get => _showExecutingWindow;
            set => Set(ref _showExecutingWindow, value);
        }

        public ObservableCollection<PHPScriptExecutor> RunningScripts 
        { 
            get => _runningScripts; 
            set => Set(ref _runningScripts, value); 
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

        public ObservableCollection<PHPDemon> Demons => _demons;

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
                foreach (var file in response.Data)
                {
                    Demons.Add(file);
                }
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
                    foreach (var demon in response.Data)
                    {
                        if (Demons.FirstOrDefault(d => d.Name == demon.Name && d.FullPath == demon.FullPath) is null)
                        {
                            Demons.Add(demon);
                        }
                    }
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
            (arg) => ConfiguredScripts is ICollection<PHPScript> { Count: > 0 } && RunningScripts is not ICollection<PHPScriptExecutor> { Count: > 0 });

        private async void OnStartScriptsExecute(object obj)
        {
            var response = _executorScriptsService.Start(ConfiguredScripts, ShowExecutingWindow);
            if (response.OperationStatus == StatusCode.Success)
            {
                await App.Current.Dispatcher.InvokeAsync(() =>
                {
                    RunningScripts = new ObservableCollection<PHPScriptExecutor>(response.Data);
                    foreach (var runner in RunningScripts)
                    {
                        runner.ScriptExitedByUser += OnScriptExitedByUser;
                        runner.ScriptOutputMessageReceived += OnScriptOutputMessageReceived;
                    }
                });
            }
        }

        public ICommand StopScriptsCommand => new RelayCommand(OnStopScriptsExecute,
            (arg) => RunningScripts is ICollection<PHPScriptExecutor> { Count: > 0 });

        private void OnStopScriptsExecute(object obj)
        {
            var response = _executorScriptsService.Stop(RunningScripts);
            if (response.OperationStatus == StatusCode.Success)
            {
                RunningScripts.Clear();
                ScriptsOutputs.Clear();
            }
        }

        #endregion

        #region --Methods--

        private async void OnScriptExitedByUser(object? sender, EventArgs e)
        {
            if (sender is PHPScriptExecutor scriptExecutor && RunningScripts.Contains(scriptExecutor))
            {
                await App.Current.Dispatcher.InvokeAsync(() =>
                {
                    var messagesToClear = ScriptsOutputs.Where(x => x.Owner.Name == scriptExecutor.ExecutableScript.Name).ToList();
                    foreach (var message in messagesToClear)
                    {
                        ScriptsOutputs.Remove(message);
                    }
                    RunningScripts.Remove(scriptExecutor);
                    scriptExecutor.Dispose();
                });
            }
        }

        private async void OnScriptOutputMessageReceived(object sender, DataReceivedEventArgs e)
        {
            if (sender is PHPScriptExecutor scriptExecutor && RunningScripts.Contains(scriptExecutor))
            {
                await App.Current.Dispatcher.InvokeAsync(() =>
                {
                    var scriptOutput = ScriptsOutputs.FirstOrDefault(s => s.Owner.Name == scriptExecutor.ExecutableScript.Name);
                    if (scriptOutput is null)
                    {
                        ScriptsOutputs.Add(new PHPScriptOutput(scriptExecutor.ExecutableScript));
                    }
                    else
                    {
                        scriptOutput.Messages.Add(e.Data!);
                    }
                });
            }
        }

        #endregion
    }
}
