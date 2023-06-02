using DemonsRunner.ViewModels.Base;
using System;

namespace DemonsRunner.ViewModels
{
    internal class MainWindowViewModel : BaseViewModel, IDisposable
    {
        #region --Fields--

        private string _windowTitle = App.Name;

        #endregion

        #region --Properties--

        public FilesPanelViewModel FilesPanelViewModel { get; }

        public WorkSpaceViewModel WorkSpaceViewModel { get; }

        public NotificationPanelViewModel NotificationPanelViewModel { get; }

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
            WorkSpaceViewModel workSpaceViewModel,
            NotificationPanelViewModel userNotificationViewModel)
        {
            WorkSpaceViewModel = workSpaceViewModel;
            FilesPanelViewModel = filesPanelViewModel;
            NotificationPanelViewModel = userNotificationViewModel;
        }

        #endregion

        #region --Commands--



        #endregion

        #region --Methods--

        public void Dispose()
        {
            NotificationPanelViewModel.Dispose();
            WorkSpaceViewModel.Dispose();
        }

        #endregion
    }
}
