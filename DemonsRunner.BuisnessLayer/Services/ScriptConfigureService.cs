using DemonsRunner.Domain.Enums;
using DemonsRunner.Domain.Models;
using System.Diagnostics;
using DemonsRunner.Domain.Responses.Intefaces;
using DemonsRunner.Domain.Responses;
using DemonsRunner.BuisnessLayer.Services.Interfaces;

namespace DemonsRunner.BuisnessLayer.Services
{
    public class ScriptConfigureService : IScriptConfigureService
    {
        public ICollectionDataResponse<PHPScript> ConfigureScripts(IEnumerable<PHPDemon> demons)
        {
            try
            {
                var configuredScripts = new List<PHPScript>();
                foreach (var demon in demons)
                {
                    configuredScripts.Add(new PHPScript(demon));
                }

                return new CollectionDataResponse<PHPScript>
                {
                    Description = "Scripts were successfully configurated!",
                    OperationStatus = StatusCode.Success,
                    Data = configuredScripts
                }; 
            }
            catch(Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return new CollectionDataResponse<PHPScript>
                {
                    Description = "Something go wrong!",
                    OperationStatus = StatusCode.Fail
                };
            }
        }
    }
}
