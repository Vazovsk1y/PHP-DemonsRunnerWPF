using DemonsRunner.Domain.Models;
using DemonsRunner.ViewModels.Base;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using DemonsRunner.BuisnessLayer.Services.Interfaces;
using DemonsRunner.Infrastructure.Messages;
using System.Windows.Controls;

namespace DemonsRunner.ViewModels
{
    internal class PHPScriptExecutorViewModel : BaseViewModel, IScriptExecutorViewModel
    {
        #region --Fields--

        private readonly IDataBus _dataBus;
        private bool _disposed = false;

        #endregion

        #region --Properties--

        public ObservableCollection<string> OutputMessages { get; } = new ObservableCollection<string>();

        public PHPScriptExecutor ScriptExecutor { get; }

        #endregion

        #region --Constructors--

        public PHPScriptExecutorViewModel(
            PHPScriptExecutor scriptExecutor, 
            IDataBus dataBus)
        {
            ScriptExecutor = scriptExecutor;
            ScriptExecutor.ScriptOutputMessageReceived += OnScriptOutputMessageReceived;
            ScriptExecutor.ScriptExitedByUser += OnScriptExitedByUser;
            _dataBus = dataBus;
        }

        #endregion

        #region --Commands--



        #endregion

        #region --Methods--

        public void Dispose() => CleanUp();

        private void OnScriptExitedByUser(object? sender, EventArgs e) => _dataBus.Send(new ScriptExitedMessage(this, e));

        private async Task OnScriptOutputMessageReceived(object sender, string message) => 
            await App.Current.Dispatcher.InvokeAsync(() => OutputMessages.Add($"[{DateTime.Now.ToShortTimeString()}]: {message!}"));

        protected async void CleanUp()
        {
            if (_disposed)
            {
                return;
            }

            ScriptExecutor.ScriptOutputMessageReceived -= OnScriptOutputMessageReceived;
            ScriptExecutor.ScriptExitedByUser -= OnScriptExitedByUser;
            ScriptExecutor.Dispose();
            await App.Current.Dispatcher.InvokeAsync(OutputMessages.Clear);
            _disposed = true;
        }

        #endregion
    }
}
