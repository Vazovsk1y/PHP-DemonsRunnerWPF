using DemonsRunner.BuisnessLayer.Services.Interfaces;
using DemonsRunner.Domain.Enums;
using DemonsRunner.Domain.Models;
using DemonsRunner.Domain.Responses.Intefaces;
using Microsoft.Extensions.Logging;

namespace DemonsRunner.BuisnessLayer.Services
{
    public class FileStateChecker : IFileStateChecker
    {
        private readonly ILogger<FileStateChecker> _logger;
        private readonly IResponseFactory _responseFactory;

        public FileStateChecker(ILogger<FileStateChecker> logger, IResponseFactory responseFactory)
        {
            _logger = logger;
            _responseFactory = responseFactory;
        }

        public Task<IResponse> IsFileExistAsync(PHPDemon file)
        {
            string messageResponse = string.Empty;
            try
            {
                _logger.LogInformation("Checking the existence of the [{fileName}] file has started", file.Name);
                var fileInfo = new FileInfo(file.FullPath);
                if (fileInfo.Exists)
                {
                    _logger.LogInformation("[{FileName}] exist in [{FileFullPath}]", file.Name, file.FullPath);
                    messageResponse = $"{file.Name} exist.";

                    return Task.FromResult(_responseFactory.CreateResponse(StatusCode.Success, messageResponse));
                }

                _logger.LogWarning("[{fileName}] isn't exist in [{fileFullPath}]", file.Name, file.FullPath);
                messageResponse = $"{file.Name} isn't exist in {file.FullPath.TrimEnd(file.Name.ToCharArray())} or was renamed";

                return Task.FromResult(_responseFactory.CreateResponse(StatusCode.Fail, messageResponse));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{ExcetptionType} was catched", typeof(Exception));
                messageResponse = "Something go wrong";

                return Task.FromResult(_responseFactory.CreateResponse(StatusCode.Fail, messageResponse));
            }
        }
    }
}
