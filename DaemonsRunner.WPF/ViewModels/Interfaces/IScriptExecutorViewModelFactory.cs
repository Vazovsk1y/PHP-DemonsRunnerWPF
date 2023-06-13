using DaemonsRunner.Domain.Models;

namespace DaemonsRunner.ViewModels.Interfaces
{
    internal interface IScriptExecutorViewModelFactory
    {
        public IScriptExecutorViewModel CreateViewModel(PHPScriptExecutor scriptExecutor);
    }
}
