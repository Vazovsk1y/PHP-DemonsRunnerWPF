using DemonsRunner.Domain.Models;

namespace DemonsRunner.ViewModels
{
    internal interface IScriptExecutorViewModelFactory
    {
        public IScriptExecutorViewModel CreateViewModel(PHPScriptExecutor scriptExecutor);
    }
}
