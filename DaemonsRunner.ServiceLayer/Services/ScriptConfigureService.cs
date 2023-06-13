using DaemonsRunner.Domain.Models;
using DaemonsRunner.BuisnessLayer.Services.Interfaces;
using DaemonsRunner.Domain.Responses;
using Microsoft.Extensions.Logging;
using DaemonsRunner.BuisnessLayer.Responses.Enums;
using DaemonsRunner.BuisnessLayer.Responses.Interfaces;
using DaemonsRunner.BuisnessLayer.Responses;

namespace DaemonsRunner.BuisnessLayer.Services
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
