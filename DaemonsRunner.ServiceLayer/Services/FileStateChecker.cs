using DaemonsRunner.BuisnessLayer.Responses.Enums;
using DaemonsRunner.BuisnessLayer.Responses.Interfaces;
using DaemonsRunner.BuisnessLayer.Services.Interfaces;
using DaemonsRunner.Domain.Models;
using DaemonsRunner.Domain.Responses;
using Microsoft.Extensions.Logging;

namespace DaemonsRunner.BuisnessLayer.Services
{
    public class FileStateChecker : IFileStateChecker
    {
        private readonly ILogger<FileStateChecker> _logger;

        public FileStateChecker(ILogger<FileStateChecker> logger)
        {
            _logger = logger;
        }

        public Task<IResponse> IsFileExistAsync(PHPFile file)
        {
            ArgumentNullException.ThrowIfNull(file);

            var response = new Response
            {
                OperationStatus = StatusCode.Success,
            };

            _logger.LogInformation("Checking the existence of the [{fileName}] file has started", file.Name);
            var fileInfo = new FileInfo(file.FullPath);

            if (!fileInfo.Exists)
            {
                _logger.LogWarning("[{fileName}] isn't exists in [{fileFullPath}]", file.Name, file.FullPath);
                response.OperationStatus = StatusCode.Fail;
                response.Description = $"{file.Name} isn't exists in {file.FullPath.TrimEnd(file.Name.ToCharArray())} or was renamed.";

                return Task.FromResult<IResponse>(response);
            }

            _logger.LogInformation("[{FileName}] exists in [{FileFullPath}]", file.Name, file.FullPath);
            response.Description = $"{file.Name} exists.";

            return Task.FromResult<IResponse>(response);
        }
    }
}
