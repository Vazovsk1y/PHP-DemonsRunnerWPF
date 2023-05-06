using DemonsRunner.Domain.Models;
using DemonsRunner.Domain.Responses.Intefaces;

namespace DemonsRunner.BuisnessLayer.Services.Interfaces
{
    public interface IScriptConfigureService
    {
        public ICollectionDataResponse<PHPScript> ConfigureScripts(IEnumerable<PHPDemon> demons);
    }
}
