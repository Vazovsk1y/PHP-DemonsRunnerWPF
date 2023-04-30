using DemonsRunner.Domain.Models;
using DemonsRunner.Domain.Responses;

namespace DemonsRunner.Domain.Services
{
    public interface IScriptConfigureService
    {
        public ICollectionDataResponse<PHPScript> ConfigureScripts(IEnumerable<PHPDemon> demons);
    }
}
