using DaemonsRunner.BuisnessLayer.Responses.Enums;
using DaemonsRunner.BuisnessLayer.Services.Interfaces;
using DaemonsRunner.Commands;
using DaemonsRunner.Domain.Models;
using DaemonsRunner.Infrastructure.Extensions;
using DaemonsRunner.ViewModels.Base;
using System.Collections;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace DaemonsRunner.ViewModels
{
    internal class FilesPanelViewModel : BaseViewModel
    {
        #region --Fields--

        private readonly IDataBus _dataBus;
        private readonly IFileService _fileService;
        private readonly IFileDialog _fileDialogService;
        private readonly ObservableCollection<PHPFile> _files = new();

        #endregion

        #region --Properties--

        public ObservableCollection<PHPFile> Files => _files;

        #endregion

        #region --Constructors--

        public FilesPanelViewModel() { }

        public FilesPanelViewModel(
            IFileService fileService,
            IFileDialog fileDialogService,
            IDataBus dataBus)
        {
            _fileService = fileService;
            _fileDialogService = fileDialogService;
            var response = _fileService.GetSaved();
            if (response.OperationStatus == StatusCode.Success)
            {
                Files.AddRange(response.Data!);
            }
            _dataBus = dataBus;
        }

        #endregion

        #region --Commands--

        public ICommand AddFileToListCommand => new RelayCommand(OnAddingFileExecute);

        public ICommand DeleteFileFromListCommand => new RelayCommand(
            OnDeletingFileExecute,
            (arg) => Files.Count > 0);

        #region --Command Handlers--

        private async void OnAddingFileExecute(object obj)
        {
            bool isCollectionModified = false;
            var response = await _fileDialogService.StartDialogAsync().ConfigureAwait(false);

            if (response.OperationStatus is StatusCode.Success)
            {
                await App.Current.Dispatcher.InvokeAsync(() =>
                {
                    isCollectionModified = Files.AddFileIfNotExist(response.Data!);
                });

                if (isCollectionModified)
                {
                    var savingResponse = _fileService.SaveAll(Files);
                    _dataBus.Send(savingResponse.Description);
                }
            }
        }

        private void OnDeletingFileExecute(object commandParametr)
        {
            var items = commandParametr as IList;
            var selectedFiles = items?.Cast<PHPFile>().ToList();

            if (selectedFiles is not null)
            {
                bool isCollectionModified = Files.RemoveAll(selectedFiles);
                if (isCollectionModified)
                {
                    var response = _fileService.SaveAll(Files);
                    _dataBus.Send(response.Description);
                }
            }
        }

        #endregion

        #endregion

        #region --Methods--



        #endregion
    }
}
