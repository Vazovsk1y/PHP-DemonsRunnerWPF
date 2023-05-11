using DemonsRunner.Domain.Models;
using DemonsRunner.ViewModels.Base;
using System.Diagnostics;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace DemonsRunner.ViewModels
{
    internal class PHPScriptExecutorViewModel : BaseViewModel, IDisposable
    {
        #region --Events--

        public event EventHandler? ScriptExited;

        #endregion

        #region --Fields--

        private bool _disposed = false;

        #endregion

        #region --Properties--

        public ObservableCollection<string> OutputMessages { get; } = new ObservableCollection<string>();

        public PHPScriptExecutor ScriptExecutor { get; }

        #endregion

        #region --Constructors--

        public PHPScriptExecutorViewModel(PHPScriptExecutor scriptExecutor)
        {
            ScriptExecutor = scriptExecutor;
            ScriptExecutor.ScriptOutputMessageReceived += OnScriptOutputMessageReceived;
            ScriptExecutor.ScriptExitedByUser += OnScriptExited;
        }

        #endregion

        #region --Commands--



        #endregion

        #region --Methods--

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private async void OnScriptExited(object? sender, EventArgs e)
        {
            if (sender is PHPScriptExecutor scriptExecutor)
            {
                await App.Current.Dispatcher.InvokeAsync(() =>
                {
                    ScriptExited?.Invoke(this, e);
                });
            }
        }

        private async Task OnScriptOutputMessageReceived(object sender, string message) => 
            await App.Current.Dispatcher.InvokeAsync(() => OutputMessages.Add($"[{DateTime.Now.ToShortTimeString()}]: {message!}"));

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    ScriptExecutor.ScriptOutputMessageReceived -= OnScriptOutputMessageReceived;
                    ScriptExecutor.ScriptExitedByUser -= OnScriptExited;
                    ScriptExecutor.Dispose();
                    OutputMessages.Clear();
                }
                _disposed = true;
            }
        }

        #endregion
    }
}
