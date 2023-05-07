using DemonsRunner.Domain.Enums;
using DemonsRunner.Domain.Models;
using System.Diagnostics;
using DemonsRunner.Domain.Responses.Intefaces;
using DemonsRunner.BuisnessLayer.Services.Interfaces;
using DemonsRunner.Domain.Responses;

namespace DemonsRunner.BuisnessLayer.Services
{
    public class ScriptConfigureService : IScriptConfigureService
    {
        public Task<IDataResponse<IEnumerable<PHPScript>>> ConfigureScripts(IEnumerable<PHPDemon> demons)
        {
            try
            {
                var configuredScripts = new List<PHPScript>();
                foreach (var demon in demons)
                {
                    configuredScripts.Add(new PHPScript(demon));
                }

                return Task.FromResult<IDataResponse<IEnumerable<PHPScript>>>(new DataResponse<IEnumerable<PHPScript>>
                {
                    Description = "Scripts were successfully configurated!",
                    OperationStatus = StatusCode.Success,
                    Data = configuredScripts
                }); 
            }
            catch(Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return Task.FromResult<IDataResponse<IEnumerable<PHPScript>>>(new DataResponse<IEnumerable<PHPScript>>
                {
                    Description = "Something go wrong!",
                    OperationStatus = StatusCode.Fail
                });
            }
        }
    }
}
