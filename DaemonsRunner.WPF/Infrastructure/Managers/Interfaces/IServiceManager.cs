using DaemonsRunner.BuisnessLayer.Responses.Interfaces;
using DaemonsRunner.Domain.Models;
using DaemonsRunner.ViewModels.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DaemonsRunner.Infrastructure.Managers.Interfaces
{
    internal interface IServiceManager
    {
        Task<(IEnumerable<IResponse> responses, IEnumerable<IScriptExecutorViewModel> successfullyStartedViewModels)> 
            GetStartingResultAsync(IEnumerable<PHPScript> configuredScripts);

        Task<(IEnumerable<IResponse> responses, IEnumerable<IScriptExecutorViewModel> successfullyStoppedViewModels)> 
            GetStoppingResultAsync(IEnumerable<IScriptExecutorViewModel> runningViewModels);

        Task<IEnumerable<IResponse>> GetStoppingResultAsync(IScriptExecutorViewModel scriptExecutorViewModel);
    }
}
