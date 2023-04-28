using DemonsRunner.Domain.Enums;
using DemonsRunner.Domain.Interfaces;
using DemonsRunner.Domain.Models;
using DemonsRunner.Domain.Services;
using DemonsRunner.BuisnessLayer.Responses;
using System.Diagnostics;

namespace DemonsRunner.BuisnessLayer.Services
{
    public class ScriptConfigureService : IScriptConfigureService
    {
        public IDataResponse<PHPScript> ConfigureScripts(IEnumerable<PHPDemon> demons)
        {
            try
            {
                var configuredScripts = new List<PHPScript>();
                foreach (var demon in demons)
                {
                    configuredScripts.Add(new PHPScript(demon));
                }

                return new DataResponse<PHPScript>
                {
                    Description = "Scripts were successfully configurated!",
                    OperationStatus = StatusCode.Success,
                    Data = configuredScripts
                }; 
            }
            catch(Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return new DataResponse<PHPScript>
                {
                    Description = "Something go wrong!",
                    OperationStatus = StatusCode.Fail
                };
            }
        }
    }
}
