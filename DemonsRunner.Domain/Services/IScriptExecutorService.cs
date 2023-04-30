using DemonsRunner.Domain.Interfaces;
using DemonsRunner.Domain.Models;
using DemonsRunner.Domain.Responses;

namespace DemonsRunner.Domain.Services
{
    public interface IScriptExecutorService
    {
        public IDataResponse<PHPScriptExecutor> Start(PHPScript script, bool showExecutingWindow);

        public IBaseResponse Stop(IEnumerable<PHPScriptExecutor> executingScripts);
    }
}
