using DemonsRunner.Domain.Models;
using System.Diagnostics;
using DemonsRunner.Domain.Enums;
using DemonsRunner.Domain.Responses.Intefaces;
using DemonsRunner.DAL.Repositories.Interfaces;
using DemonsRunner.Domain.Responses;
using DemonsRunner.BuisnessLayer.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace DemonsRunner.BuisnessLayer.Services
{
    public class FileService : IFileService
    {
        private readonly IFileRepository<PHPDemon> _repository;
        private readonly ILogger<FileService> _logger;
        private readonly IResponseFactory _responseFactory;

        public FileService(
            IFileRepository<PHPDemon> repository, 
            ILogger<FileService> logger, 
            IResponseFactory responseFactory)
        {
            _repository = repository;
            _logger = logger;
            _responseFactory = responseFactory;
        }

        public IDataResponse<IEnumerable<PHPDemon>> GetSaved()
        {
            var messageResponse = string.Empty;
            try
            {
                _logger.LogInformation("Getting saved files from the storage .json file started");
                var files = _repository.GetAll();

                if (files is null)
                {
                    _logger.LogError("Storage [data.json] file wasn't founded or was renamed!");
                    messageResponse = "Storage [data.json] file was't founded.";

                    return _responseFactory.CreateDataResponse(StatusCode.Fail, messageResponse, Enumerable.Empty<PHPDemon>());
                }

                if (files.ToList().Count is 0)
                {
                    _logger.LogWarning($"Received files count was [0]");
                    messageResponse = "Storage file was empty.";

                    return _responseFactory.CreateDataResponse(StatusCode.Fail, messageResponse, Enumerable.Empty<PHPDemon>());
                }

                _logger.LogInformation("Received files count [{filesCount}]", files.ToList().Count);
                messageResponse = $"{files.ToList().Count} files were succsessfully given to you!";

                return _responseFactory.CreateDataResponse(StatusCode.Success, messageResponse, files);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "{ExcetptionType} was catched", typeof(Exception));
                messageResponse = "Something go wrong";

                return _responseFactory.CreateDataResponse(StatusCode.Fail, messageResponse, Enumerable.Empty<PHPDemon>());
            }
        }

        public IResponse SaveAll(IEnumerable<PHPDemon> savedFiles)
        {
            string messageResponse = string.Empty;
            try
            {
                int savedFilesCount = savedFiles.ToList().Count;
                _logger.LogInformation("Saving [{savedFilesCount}] files in storage file started", savedFilesCount);
                _repository.SaveAll(savedFiles);
                _logger.LogInformation("[{savedFilesCount}] files were succsessfully saved", savedFilesCount);
                messageResponse = $"{savedFilesCount} files were succsessfully saved";

                return _responseFactory.CreateResponse(StatusCode.Success, messageResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{ExcetptionType} was catched", typeof(Exception));
                messageResponse = "Something go wrong";

                return _responseFactory.CreateResponse(StatusCode.Fail, messageResponse);
            }
        }
    }
}
