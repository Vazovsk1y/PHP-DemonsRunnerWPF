using DemonsRunner.Domain.Models;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace DemonsRunner.ViewModels
{
    internal interface IScriptExecutorViewModel : IDisposable, INotifyPropertyChanged
    {
        public ObservableCollection<string> OutputMessages { get; }

        public PHPScriptExecutor ScriptExecutor { get; }
    }
}
