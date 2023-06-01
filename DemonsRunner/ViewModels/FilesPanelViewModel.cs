using DemonsRunner.BuisnessLayer.Services.Interfaces;
using DemonsRunner.Commands;
using DemonsRunner.Domain.Enums;
using DemonsRunner.Domain.Models;
using DemonsRunner.Infrastructure.Extensions;
using DemonsRunner.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows.Input;

namespace DemonsRunner.ViewModels
{
    internal class FilesPanelViewModel : BaseViewModel
    {
        #region --Fields--

        private readonly ObservableCollection<SelectionViewModel<PHPFile>> _filesViewModels = new();
        private readonly IFileService _fileService;
        private readonly IFileDialogService _fileDialogService;
        private readonly IDataBus _dataBus;

        #endregion

        #region --Properties--

        public ObservableCollection<SelectionViewModel<PHPFile>> FilesViewModels => _filesViewModels;

        public ICollection<PHPFile> SelectedFiles => FilesViewModels.Where(v => v.IsSelected).Select(i => i.Value).ToList();

        public ICollection<PHPFile> Files => FilesViewModels.Select(i => i.Value).ToList();

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
                var viewModels = new List<SelectionViewModel<PHPFile>>();
                foreach (var file in response.Data!)
                {
                    viewModels.Add(new SelectionViewModel<PHPFile>(file));
                }
                FilesViewModels.AddRange(viewModels);
            }
            _dataBus = dataBus;
        }

        #endregion

        #region --Commands--

        public ICommand AddFileToListCommand => new RelayCommand(OnAddingFileExecute);

        private async void OnAddingFileExecute(object obj)
        {
            bool isCollectionModified = false;
            var response = await _fileDialogService.StartDialogAsync().ConfigureAwait(false);

            if (response.OperationStatus == StatusCode.Success)
            {
                await App.Current.Dispatcher.InvokeAsync(() =>
                {
                    foreach (var file in response.Data!)
                    {
                        if (Files.FirstOrDefault(d => d.Name == file.Name && d.FullPath == file.FullPath) is null)
                        {
                            isCollectionModified = true;
                            FilesViewModels.Add(new SelectionViewModel<PHPFile>(file));
                        }
                    }
                });

                if (isCollectionModified)
                {
                    var savingResponse = _fileService.SaveAll(Files);
                    _dataBus.Send(savingResponse.Description);
                }
            }
        }

        public ICommand DeleteFileFromListCommand => new RelayCommand(OnDeletingFileExecute,
            (arg) => Files.Count > 0 && SelectedFiles.Count > 0);

        private void OnDeletingFileExecute(object obj)
        {
            foreach(var selectedFile in SelectedFiles)
            {
                var viewModelToDelete = FilesViewModels.FirstOrDefault(i => i.Value.Name == selectedFile.Name);
                if (viewModelToDelete is not null)
                {
                    FilesViewModels.Remove(viewModelToDelete);
                }
            }
            var response = _fileService.SaveAll(Files);
            _dataBus.Send(response.Description);
        }

        #endregion

        #region --Methods--
       


        #endregion
    }

    internal class SelectionViewModel<T> : BaseViewModel
    {
        private bool _isSelected;

        public SelectionViewModel(T value)
        {
            Value = value;
        }

        public T Value { get; }

        public bool IsSelected
        {
            get => _isSelected;
            set => Set(ref _isSelected, value);
        }
    }
}
