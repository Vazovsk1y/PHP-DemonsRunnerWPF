using DemonsRunner.Commands;
using DemonsRunner.Implementations.Services;
using DemonsRunner.Interfaces;
using DemonsRunner.Models;
using DemonsRunner.ViewModels.Base;
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
        private readonly IFileDialogService dialogService = new FileDialogService();

        #endregion

        #region --Properties--

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
            var response = dialogService.StartDialog();

            if (response.OperationStatus == Enums.StatusCode.Success)
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

        public ICommand DeleteFileFromListCommand => new RelayCommand(OnFileDeletingExecute, 
            (arg) => Demons.Count > 0 && SelectedDemon is not null);

        private void OnFileDeletingExecute(object obj)
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
