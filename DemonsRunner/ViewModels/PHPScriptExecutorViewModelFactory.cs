using DemonsRunner.BuisnessLayer.Services.Interfaces;
using DemonsRunner.Domain.Models;

namespace DemonsRunner.ViewModels
{
    internal class PHPScriptExecutorViewModelFactory : IScriptExecutorViewModelFactory
    {
        private readonly IDataBus _dataBus;

        public PHPScriptExecutorViewModelFactory(IDataBus dataBus)
        {
            _dataBus = dataBus;
        }

        public IScriptExecutorViewModel CreateViewModel(PHPScriptExecutor scriptExecutor)
        {
            return new PHPScriptExecutorViewModel(scriptExecutor, _dataBus);
        }
    }
}
