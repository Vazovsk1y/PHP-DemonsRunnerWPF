using DemonsRunner.Domain.Enums;
using DemonsRunner.Domain.Models;
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
            ArgumentNullException.ThrowIfNull(demons);

            _logger.LogInformation("Configuring [{demonsCount}] files in scripts started", demons.ToList().Count);
            var configuredScripts = new List<PHPScript>();
            foreach (var demon in demons)
            {
                configuredScripts.Add(new PHPScript(demon));
            }
            _logger.LogInformation("[{configuredScriptsCount}] scripts were successfully configured", configuredScripts.Count);

            var response = new DataResponse<IEnumerable<PHPScript>>
            {
                OperationStatus = StatusCode.Success,
                Data = configuredScripts,
                Description = $"[{configuredScripts.Count}] scripts were successfully configured."
            };
            return Task.FromResult<IDataResponse<IEnumerable<PHPScript>>>(response);
        }
    }
}
