using DemonsRunner.Commands;
using DemonsRunner.Domain.Enums;
using DemonsRunner.Domain.Interfaces;
using DemonsRunner.Domain.Models;
using DemonsRunner.Domain.Services;
using DemonsRunner.Implementations.Services;
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
        private PHPDemon _selectedDemon;
        private readonly ObservableCollection<PHPDemon> _demons = new();
        private ObservableCollection<PHPScript> _scripts;
        private readonly IFileDialogService _dialogService = new FileDialogService();
        private readonly IScriptConfigureService _configureService = new ScriptConfigureService();

        #endregion

        #region --Properties--

        public ObservableCollection<PHPScript> Scripts 
        { 
            get => _scripts; 
            set => Set(ref _scripts, value);
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
            var response = _configureService.ConfigureScripts(Demons);
            if (response.OperationStatus == StatusCode.Success)
            {
                await App.Current.Dispatcher.InvokeAsync(() =>
                {
                    Scripts = new ObservableCollection<PHPScript>(response.Data);
                });
            }
        }

        public ICommand ClearConfigureScripts => new RelayCommand(OnClearConfiguredScriptsExecute, 
            (arg) => Scripts is ICollection<PHPScript> { Count: > 0 });

        private void OnClearConfiguredScriptsExecute(object obj) => Scripts.Clear();

        #endregion

        #region --Methods--



        #endregion
    }
}
