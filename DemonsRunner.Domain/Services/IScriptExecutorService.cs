using DemonsRunner.Domain.Interfaces;
using DemonsRunner.Domain.Models;

namespace DemonsRunner.Domain.Services
{
    public interface IScriptExecutorService
    {
        public IDataResponse<PHPScriptExecutor> Start(IEnumerable<PHPScript> scripts, bool showExecutingWindow);

        public IDataResponse<PHPScriptExecutor> Stop(IEnumerable<PHPScriptExecutor> executingScripts);
    }
}
