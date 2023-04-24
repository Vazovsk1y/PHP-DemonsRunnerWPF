using DemonsRunner.Domain.Interfaces;
using DemonsRunner.Domain.Models;

namespace DemonsRunner.Domain.Services
{
    public interface IScriptExecutorService
    {
        public IResponse<PHPScriptExecutor> Start(IEnumerable<PHPScript> scripts, bool showExecutingWindow);

        public IResponse<PHPScriptExecutor> Stop(IEnumerable<PHPScriptExecutor> executingScripts);
    }
}
