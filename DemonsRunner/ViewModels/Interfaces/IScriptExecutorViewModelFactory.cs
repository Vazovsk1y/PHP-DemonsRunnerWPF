using DemonsRunner.Domain.Models;

namespace DemonsRunner.ViewModels.Interfaces
{
    internal interface IScriptExecutorViewModelFactory
    {
        public IScriptExecutorViewModel CreateViewModel(PHPScriptExecutor scriptExecutor);
    }
}
