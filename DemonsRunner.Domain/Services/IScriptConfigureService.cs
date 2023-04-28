using DemonsRunner.Domain.Interfaces;
using DemonsRunner.Domain.Models;

namespace DemonsRunner.Domain.Services
{
    public interface IScriptConfigureService
    {
        public IDataResponse<PHPScript> ConfigureScripts(IEnumerable<PHPDemon> demons);
    }
}
