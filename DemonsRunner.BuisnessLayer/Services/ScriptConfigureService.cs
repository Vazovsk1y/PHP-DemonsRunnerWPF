using DemonsRunner.Domain.Models;
using DemonsRunner.BuisnessLayer.Services.Interfaces;
using DemonsRunner.Domain.Responses;
using Microsoft.Extensions.Logging;
using DemonsRunner.BuisnessLayer.Responses.Enums;
using DemonsRunner.BuisnessLayer.Responses.Interfaces;
using DemonsRunner.BuisnessLayer.Responses;

namespace DemonsRunner.BuisnessLayer.Services
{
    public class ScriptConfigureService : IScriptConfigureService
    {
        private readonly ILogger<ScriptConfigureService> _logger;

        public ScriptConfigureService(ILogger<ScriptConfigureService> logger)
        {
            _logger = logger;
        }

        public Task<IDataResponse<IEnumerable<PHPScript>>> ConfigureScriptsAsync(IEnumerable<PHPFile> phpFiles)
        {
            ArgumentNullException.ThrowIfNull(phpFiles);

            _logger.LogInformation("Configuring [{demonsCount}] files in scripts started", phpFiles.ToList().Count);
            var configuredScripts = new List<PHPScript>();
            foreach (var demon in phpFiles)
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
