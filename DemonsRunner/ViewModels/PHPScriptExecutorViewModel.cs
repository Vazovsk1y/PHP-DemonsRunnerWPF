using DemonsRunner.Domain.Models;
using DemonsRunner.ViewModels.Base;
using System.Diagnostics;
using System;
using System.Collections.ObjectModel;

namespace DemonsRunner.ViewModels
{
    internal class PHPScriptExecutorViewModel : BaseViewModel, IDisposable
    {
        #region --Fields--



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
            ScriptExecutor.ScriptExitedByUser += OnScriptExitedByUser;
        }

        #endregion

        #region --Commands--



        #endregion

        #region --Methods--

        public void Dispose() => ScriptExecutor.Dispose();

        private async void OnScriptExitedByUser(object? sender, EventArgs e)
        {
            if (sender is PHPScriptExecutor scriptExecutor)
            {
                await App.Current.Dispatcher.InvokeAsync(scriptExecutor.Dispose);
            }
        }

        private async void OnScriptOutputMessageReceived(object sender, DataReceivedEventArgs e)
        {
            if (sender is PHPScriptExecutor scriptExecutor)
            {
                await App.Current.Dispatcher.InvokeAsync(() => OutputMessages.Add(e.Data!));
            }
        }

        #endregion
    }
}
