using DemonsRunner.Domain.Enums;
using DemonsRunner.Domain.Models;
using DemonsRunner.Domain.Services;
using DemonsRunner.BuisnessLayer.Responses;
using System.Diagnostics;
using DemonsRunner.Domain.Responses;

namespace DemonsRunner.BuisnessLayer.Services
{
    public class ScriptConfigureService : IScriptConfigureService
    {
        public Task<ICollectionDataResponse<PHPScript>> ConfigureScripts(IEnumerable<PHPDemon> demons)
        {
            try
            {
                var configuredScripts = new List<PHPScript>();
                foreach (var demon in demons)
                {
                    configuredScripts.Add(new PHPScript(demon));
                }

                return Task.FromResult<ICollectionDataResponse<PHPScript>>(new CollectionDataResponse<PHPScript>
                {
                    Description = "Scripts were successfully configurated!",
                    OperationStatus = StatusCode.Success,
                    Data = configuredScripts
                }); 
            }
            catch(Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return Task.FromResult<ICollectionDataResponse<PHPScript>>(new CollectionDataResponse<PHPScript>
                {
                    Description = "Something go wrong!",
                    OperationStatus = StatusCode.Fail
                });
            }
        }
    }
}
