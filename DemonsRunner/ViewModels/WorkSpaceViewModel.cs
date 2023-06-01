﻿using DemonsRunner.BuisnessLayer.Services.Interfaces;
using DemonsRunner.Commands;
using DemonsRunner.Domain.Enums;
using DemonsRunner.Domain.Models;
using DemonsRunner.Infrastructure.Extensions;
using DemonsRunner.Infrastructure.Managers.Interfaces;
using DemonsRunner.Infrastructure.Messages;
using DemonsRunner.ViewModels.Base;
using DemonsRunner.ViewModels.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace DemonsRunner.ViewModels
{
    internal class WorkSpaceViewModel : BaseViewModel, IDisposable
    {
        #region --Fields--

        private readonly IDataBus _dataBus;
        private readonly IDisposable _subscription;
        private readonly FilesPanelViewModel _filesPanelViewModel;
        private readonly IServiceManager _serviceManager;
        private readonly IScriptConfigureService _configureSctiptsService;
        private readonly ObservableCollection<IScriptExecutorViewModel> _runningScriptsViewModels = new();

        private ObservableCollection<PHPScript> _configuredScripts;
        private bool? _isStartButtonEnable = null;
        private bool? _isStopButtonEnable = null;

        #endregion

        #region --Properties--

        public bool? IsStartButtonEnable
        {
            get
            {
                bool defaultCondition = ConfiguredScripts is ICollection<PHPScript> { Count: > 0 } &&
                      RunningScriptsViewModels is ICollection<IScriptExecutorViewModel> { Count: 0 };

                return _isStartButtonEnable is bool condition
                    ? condition
                    : defaultCondition;
            }
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
            IServiceManager serviceManager,
            IDataBus dataBus)
        {
            _filesPanelViewModel = filesPanelViewModel;
            _configureSctiptsService = configureSctiptsService;
            _serviceManager = serviceManager;
            _dataBus = dataBus;
            _subscription = _dataBus.RegisterHandler<ScriptExitedMessage>(OnScriptExited);
        }

        #endregion

        #region --Commands--

        public ICommand ConfigureScriptsCommand => new RelayCommand(
            OnConfigureScriptsExecute,
            (arg) => _filesPanelViewModel.Files.Count > 0);

        public ICommand ClearConfigureScripts => new RelayCommand(
            (arg) => ConfiguredScripts.Clear(),
            (arg) => ConfiguredScripts is ICollection<PHPScript> { Count: > 0 });

        public ICommand StartScriptsCommand => new RelayCommand(
            OnStartScriptsExecute,
            (arg) => (bool)IsStartButtonEnable!);

        public ICommand StopScriptsCommand => new RelayCommand(
            OnStopScriptsExecute,
            (arg) => (bool)IsStopButtonEnable!);

        #region -Commands Handlers-

        private async void OnConfigureScriptsExecute(object obj)
        {
            var response = await _configureSctiptsService.ConfigureScripts(_filesPanelViewModel.Files).ConfigureAwait(false);
            if (response.OperationStatus == StatusCode.Success)
            {
                await App.Current.Dispatcher.InvokeAsync(() =>
                {
                    ConfiguredScripts = new ObservableCollection<PHPScript>(response.Data!);
                });
            }
            _dataBus.Send(response.Description);
        }

        private async void OnStartScriptsExecute(object obj)
        {
            IsStartButtonEnable = false;
            var (responses, executorsViewModels) = await _serviceManager.GetStartingResultAsync(ConfiguredScripts).ConfigureAwait(false);
            var failedResponses = responses.Where(r => r.OperationStatus is StatusCode.Fail).ToList();

            if (failedResponses.Count is 0)
            {
                _dataBus.Send($"{executorsViewModels.ToList().Count} scripts were successfully started.");
            }
            else
            {
                _dataBus.SendDescriptions(failedResponses);
            }

            await App.Current.Dispatcher.InvokeAsync(() =>
            {
                RunningScriptsViewModels.AddRange(executorsViewModels);
            });
            IsStopButtonEnable = null;
        }

        private async void OnStopScriptsExecute(object obj)
        {
            IsStopButtonEnable = false;
            var (responses, successfullyStoppedViewModels) = await _serviceManager.GetStoppingResultAsync(RunningScriptsViewModels).ConfigureAwait(false);
            var failedResponses = responses.Where(r => r.OperationStatus is StatusCode.Fail).ToList();

            if (failedResponses.Count is 0)
            {
                _dataBus.Send($"All scripts were succsessfully stopped.");
            }
            else
            {
                _dataBus.SendDescriptions(failedResponses);
            }

            await App.Current.Dispatcher.InvokeAsync(() =>
            {
                RunningScriptsViewModels.RemoveAll(successfullyStoppedViewModels);
            });
            IsStartButtonEnable = null;
        }

        #endregion

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

        #region --Databus messages handlers--

        private async void OnScriptExited(ScriptExitedMessage message)
        {
            await App.Current.Dispatcher.InvokeAsync(() =>
            {
                RunningScriptsViewModels.Remove(message.Sender);
            });
            IsStopButtonEnable = null;
            IsStartButtonEnable = null;
        }

        #endregion

        #endregion
    }
}
