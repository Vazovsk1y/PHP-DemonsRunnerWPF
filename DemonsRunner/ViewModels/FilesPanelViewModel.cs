using DemonsRunner.Commands;
using DemonsRunner.Domain.Enums;
using DemonsRunner.Domain.Interfaces;
using DemonsRunner.Domain.Models;
using DemonsRunner.Domain.Services;
using DemonsRunner.Infrastructure.Extensions;
using DemonsRunner.ViewModels.Base;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace DemonsRunner.ViewModels
{
    internal class FilesPanelViewModel : BaseViewModel
    {
        #region --Fields--

        private PHPDemon _selectedDemon;
        private readonly ObservableCollection<PHPDemon> _demons = new();
        private readonly IFileService _fileService;
        private readonly IFileDialogService _fileDialogService;

        #endregion

        #region --Properties--

        public ObservableCollection<PHPDemon> Demons => _demons;

        public PHPDemon SelectedDemon
        {
            get => _selectedDemon;
            set => Set(ref _selectedDemon, value);
        }

        #endregion

        #region --Constructors--

        public FilesPanelViewModel() { }

        public FilesPanelViewModel(
            IFileService fileService,
            IFileDialogService fileDialogService)
        {
            _fileService = fileService;
            _fileDialogService = fileDialogService;
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
            var response = _fileDialogService.StartDialog();
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

        #endregion

        #region --Methods--



        #endregion
    }
}
