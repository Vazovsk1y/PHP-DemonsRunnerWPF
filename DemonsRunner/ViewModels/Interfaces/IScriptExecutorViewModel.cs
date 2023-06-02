using DemonsRunner.Domain.Models;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace DemonsRunner.ViewModels.Interfaces
{
    internal interface IScriptExecutorViewModel : IDisposable, INotifyPropertyChanged
    {
        public ObservableCollection<string> OutputMessages { get; }

        public PHPScriptExecutor ScriptExecutor { get; }
    }
}
