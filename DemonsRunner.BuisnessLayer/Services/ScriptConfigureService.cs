using DemonsRunner.Domain.Enums;
using DemonsRunner.Domain.Models;
using System.Diagnostics;
using DemonsRunner.Domain.Responses.Intefaces;
using DemonsRunner.BuisnessLayer.Services.Interfaces;
using DemonsRunner.Domain.Responses;
using Microsoft.Extensions.Logging;

namespace DemonsRunner.BuisnessLayer.Services
{
    public class ScriptConfigureService : IScriptConfigureService
    {
        private readonly ILogger<ScriptConfigureService> _logger;

        public ScriptConfigureService(ILogger<ScriptConfigureService> logger) 
        {
            _logger = logger;
        }

        public Task<IDataResponse<IEnumerable<PHPScript>>> ConfigureScripts(IEnumerable<PHPDemon> demons)
        {
            try
            {
                _logger.LogInformation("Configuring {demonsCount} files in script started", demons?.ToList().Count);
                var configuredScripts = new List<PHPScript>();

                foreach (var demon in demons)
                {
                    configuredScripts.Add(new PHPScript(demon));
                }

                _logger.LogInformation("{configuredScriptsCount} scripts were successfully configured", configuredScripts.Count);
                return Task.FromResult<IDataResponse<IEnumerable<PHPScript>>>(new DataResponse<IEnumerable<PHPScript>>
                {
                    Description = "Scripts were successfully configurated!",
                    OperationStatus = StatusCode.Success,
                    Data = configuredScripts
                }); 
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "{ExcetptionType} was catched", typeof(Exception));
                return Task.FromResult<IDataResponse<IEnumerable<PHPScript>>>(new DataResponse<IEnumerable<PHPScript>>
                {
                    Description = "Something go wrong!",
                    OperationStatus = StatusCode.Fail
                });
            }
        }
    }
}
