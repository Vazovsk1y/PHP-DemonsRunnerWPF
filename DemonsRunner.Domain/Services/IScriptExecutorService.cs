using DemonsRunner.Domain.Interfaces;
using DemonsRunner.Domain.Models;
using DemonsRunner.Domain.Responses;

namespace DemonsRunner.Domain.Services
{
    public interface IScriptExecutorService
    {
        public Task<IDataResponse<PHPScriptExecutor>> StartExecutingAsync(PHPScript script, bool showExecutingWindow);

        public Task<IBaseResponse> StopAsync(PHPScriptExecutor executingScript);
    }
}
