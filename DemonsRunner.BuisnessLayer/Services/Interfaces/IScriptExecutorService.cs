using DemonsRunner.Domain.Models;
using DemonsRunner.Domain.Responses.Intefaces;

namespace DemonsRunner.BuisnessLayer.Services.Interfaces
{
    public interface IScriptExecutorService
    {
        public Task<IDataResponse<PHPScriptExecutor>> StartExecutingAsync(PHPScript script, bool showExecutingWindow);

        public Task<IResponse> StopAsync(PHPScriptExecutor executingScript);
    }
}
