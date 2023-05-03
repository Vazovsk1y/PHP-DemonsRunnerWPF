using DemonsRunner.ViewModels.Base;

namespace DemonsRunner.ViewModels
{
    internal class MainWindowViewModel : BaseViewModel
    {
        #region --Fields--

        private string _windowTitle = "Main";

        #endregion

        #region --Properties--

        public FilesPanelViewModel FilesPanelViewModel { get; }

        public WorkSpaceViewModel WorkSpaceViewModel { get; }

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
            WorkSpaceViewModel workSpaceViewModel)
        {
            WorkSpaceViewModel = workSpaceViewModel;
            FilesPanelViewModel = filesPanelViewModel;
        }

        #endregion

        #region --Commands--



        #endregion

        #region --Methods--



        #endregion
    }
}
