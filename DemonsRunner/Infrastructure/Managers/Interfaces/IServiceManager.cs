using DemonsRunner.Domain.Models;
using DemonsRunner.Domain.Responses.Intefaces;
using DemonsRunner.ViewModels.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DemonsRunner.Infrastructure.Managers.Interfaces
{
    internal interface IServiceManager
    {
        Task<(IEnumerable<IResponse> responses, IEnumerable<IScriptExecutorViewModel> successfullyStartedViewModels)> 
            GetStartingResultAsync(IEnumerable<PHPScript> configuredScripts);

        Task<(IEnumerable<IResponse> responses, IEnumerable<IScriptExecutorViewModel> successfullyStoppedViewModels)> 
            GetStoppingResultAsync(IEnumerable<IScriptExecutorViewModel> runningViewModels);
    }
}
