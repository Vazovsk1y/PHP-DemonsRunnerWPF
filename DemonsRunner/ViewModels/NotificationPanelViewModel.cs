using DemonsRunner.BuisnessLayer.Services.Interfaces;
using DemonsRunner.Commands;
using DemonsRunner.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace DemonsRunner.ViewModels
{
    internal class NotificationPanelViewModel : BaseViewModel, IDisposable
    {
        #region --Fields--

        private readonly ObservableCollection<string> _notifications = new();
        private readonly ICollection<IDisposable> _subscriptions = new List<IDisposable>();
        private readonly IDataBus _dataBus;

        #endregion

        #region --Properties--

        public ObservableCollection<string> Notifications => _notifications;

        #endregion

        #region --Constructors--

        public NotificationPanelViewModel() { }

        public NotificationPanelViewModel(IDataBus dataBus)
        {
            _dataBus = dataBus;
            _subscriptions.Add(_dataBus.RegisterHandler<string>(OnMessageReceived));
        }

        #endregion

        #region --Commands--

        public ICommand ClearNotificationsCommand => new RelayCommand(
            (arg) => Notifications.Clear(),
            (arg) => Notifications.Count > 0);

        #endregion

        #region --Methods--

        public void Dispose() 
        {
            foreach (var subscription in _subscriptions)
            {
                subscription.Dispose();
            }
        }

        private async void OnMessageReceived(string message)
        {
            await App.Current.Dispatcher.InvokeAsync(() => Notifications.Add($"[{DateTime.Now.ToShortTimeString()}]: {message}"));
        }

        #endregion
    }
}
