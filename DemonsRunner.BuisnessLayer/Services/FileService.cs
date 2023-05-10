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

        public FileService(IFileRepository<PHPDemon> repository, ILogger<FileService> logger, IResponseFactory responseFactory)
        {
            _repository = repository;
            _logger = logger;
            _responseFactory = responseFactory;
        }

        public IDataResponse<IEnumerable<PHPDemon>> GetSaved()
        {
            try
            {
                _logger.LogInformation("Getting saved files from the storage .json file started");
                var files = _repository.GetAll();

                if (files is null)
                {
                    _logger.LogError("Storage json file not founded!");
                    return _responseFactory.CreateDataResponse(StatusCode.Fail, "Storage json file not founded!", Enumerable.Empty<PHPDemon>());
                }

                if (files.ToList().Count is 0)
                {
                    _logger.LogWarning($"Received files count was 0");
                    return _responseFactory.CreateDataResponse(StatusCode.Fail, "Storage file was empty", Enumerable.Empty<PHPDemon>());
                }

                _logger.LogInformation("Received files count [{filesCount}]", files.ToList().Count);
                return _responseFactory.CreateDataResponse(StatusCode.Success, $"{files.ToList().Count} files were succsessfully given to you!", files);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "{ExcetptionType} was catched", typeof(Exception));
                return _responseFactory.CreateDataResponse(StatusCode.Fail, "Something go wrong", Enumerable.Empty<PHPDemon>());
            }
        }

        public IResponse IsFileExist(PHPDemon file)
        {
            try
            {
                _logger.LogInformation("Checking the existence of the [{fileName}] file has started", file.Name);
                var fileInfo = new FileInfo(file.FullPath);
                if (fileInfo.Exists)
                {
                    _logger.LogInformation("[{FileName}] exist in [{FileFullPath}]", file.Name, file.FullPath);
                    return _responseFactory.CreateResponse(StatusCode.Success, "File exist!");
                }

                _logger.LogWarning("[{fileName}] isn't exist in [{fileFullPath}]", file.Name, file.FullPath);
                return _responseFactory.CreateResponse(StatusCode.Fail, "File isn't exist!");
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "{ExcetptionType} was catched", typeof(Exception));
                return _responseFactory.CreateResponse(StatusCode.Fail, "Something go wrong");
            }
        }

        public IResponse SaveAll(IEnumerable<PHPDemon> savedFiles)
        {
            try
            {
                _logger.LogInformation("Saving [{savedFilesCount}] files in storage file started", savedFiles.ToList().Count);
                _repository.SaveAll(savedFiles);
                _logger.LogInformation("[{savedFilesCount}] files were succsessfully saved", savedFiles.ToList().Count);
                return _responseFactory.CreateResponse(StatusCode.Success, "Files were succsessfully saved");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{ExcetptionType} was catched", typeof(Exception));
                return _responseFactory.CreateResponse(StatusCode.Fail, "Something go wrong");
            }
        }
    }
}
