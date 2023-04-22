using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace DemonsRunner.ViewModels.Base
{
    internal class BaseViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        public BaseViewModel() { }

        protected virtual void OnPropertyChanged([CallerMemberName] string callerName = null) => 
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(callerName));

        protected virtual bool Set<T>(ref T field, T value, [CallerMemberName]string callerName = null)
        {
            if (Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(callerName);
            return true;
        }
    }
}
