using DemonsRunner.BuisnessLayer.Services.Interfaces;
using DemonsRunner.Commands;
using DemonsRunner.Domain.Enums;
using DemonsRunner.Domain.Models;
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
        private readonly IDataBus _dataBus;

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
            IFileDialogService fileDialogService,
            IDataBus dataBus)
        {
            _fileService = fileService;
            _fileDialogService = fileDialogService;
            var response = _fileService.GetSaved();
            if (response.OperationStatus == StatusCode.Success)
            {
                Demons.AddRange(response.Data!);
            }
            _dataBus = dataBus;
            _dataBus.Send(response.Description);
        }

        #endregion

        #region --Commands--

        public ICommand AddFileToListCommand => new RelayCommand(OnAddingFileExecute);

        private async void OnAddingFileExecute(object obj)
        {
            bool isCollectionModified = false;
            var response = await _fileDialogService.StartDialog().ConfigureAwait(false);
            if (response.OperationStatus == StatusCode.Success)
            {
                await App.Current.Dispatcher.InvokeAsync(() =>
                {
                    isCollectionModified = Demons.AddIfNotExist(response.Data);
                });

                if (isCollectionModified)
                {
                    _fileService.SaveAll(Demons);
                }
            }
            _dataBus.Send(response.Description);
        }

        public ICommand DeleteFileFromListCommand => new RelayCommand(OnDeletingFileExecute,
            (arg) => Demons.Count > 0 && SelectedDemon is not null);

        private void OnDeletingFileExecute(object obj)
        {
            if (Demons.Contains(SelectedDemon))
            {
                Demons.Remove(SelectedDemon);
                SelectedDemon = null;
                _fileService.SaveAll(Demons);
            }
        }

        #endregion

        #region --Methods--



        #endregion
    }
}
