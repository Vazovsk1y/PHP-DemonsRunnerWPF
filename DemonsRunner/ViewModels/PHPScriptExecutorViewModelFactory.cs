using DemonsRunner.BuisnessLayer.Services.Interfaces;
using DemonsRunner.Domain.Models;
using DemonsRunner.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
