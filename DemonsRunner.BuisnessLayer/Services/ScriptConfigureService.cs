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
        private readonly IResponseFactory _responseFactory;

        public ScriptConfigureService(ILogger<ScriptConfigureService> logger, IResponseFactory responseFactory)
        {
            _logger = logger;
            _responseFactory = responseFactory;
        }

        public Task<IDataResponse<IEnumerable<PHPScript>>> ConfigureScripts(IEnumerable<PHPDemon> demons)
        {
            string messageResponse = string.Empty;
            try
            {
                _logger.LogInformation("Configuring [{demonsCount}] files in scripts started", demons.ToList().Count);
                var configuredScripts = new List<PHPScript>();

                foreach (var demon in demons)
                {
                    configuredScripts.Add(new PHPScript(demon));
                }

                _logger.LogInformation("[{configuredScriptsCount}] scripts were successfully configured", configuredScripts.Count);
                messageResponse = "All files were successfully configured.";

                return Task.FromResult(_responseFactory.CreateDataResponse(StatusCode.Success, messageResponse, configuredScripts.AsEnumerable()));
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "{ExcetptionType} was catched", typeof(Exception));
                messageResponse = "Something go wrong!";

                return Task.FromResult(_responseFactory.CreateDataResponse(StatusCode.Fail, messageResponse, Enumerable.Empty<PHPScript>()));
            }
        }
    }
}
