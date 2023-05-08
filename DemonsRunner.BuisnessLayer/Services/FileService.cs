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

        public FileService(IFileRepository<PHPDemon> repository, ILogger<FileService> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public IDataResponse<IEnumerable<PHPDemon>> GetSaved()
        {
            try
            {
                _logger.LogInformation("Getting saved files from the storage .json file started");
                var files = _repository.GetAll().ToList();

                if (files is null)
                {
                    _logger.LogError("Storage json file not founded!");
                    return new DataResponse<IEnumerable<PHPDemon>>
                    {
                        Description = "Storage json file not founded!",
                        OperationStatus = StatusCode.Fail,
                    };
                }

                if (files.Count is 0)
                {
                    _logger.LogWarning($"Received files count was 0");
                    return new DataResponse<IEnumerable<PHPDemon>>
                    {
                        Description = "Storage file was empty",
                        OperationStatus = StatusCode.Fail,
                    };
                }

                _logger.LogInformation("Received files count {filesCount}", files.Count);
                return new DataResponse<IEnumerable<PHPDemon>>
                {
                    Description = $"{files.Count} files were succsessfully given to you!",
                    OperationStatus = StatusCode.Success,
                    Data = files
                };
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "{ExcetptionType} was catched", typeof(Exception));
                return new DataResponse<IEnumerable<PHPDemon>>
                {
                    Description = "Something go wrong",
                    OperationStatus = StatusCode.Fail,
                };
            }
        }

        public IResponse IsFileExist(PHPDemon file)
        {
            try
            {
                _logger.LogInformation("Checking the existence of the file has started");
                var fileInfo = new FileInfo(file.FullPath);
                if (fileInfo.Exists)
                {
                    _logger.LogInformation("{FileName} exist in {FileFullPath}", file.Name, file.FullPath);
                    return new Response
                    {
                        Description = "File exist!",
                        OperationStatus = StatusCode.Success,
                    };
                }

                _logger.LogWarning("{fileName} isn't exist in {fileFullPath}", file.Name, file.FullPath);
                return new Response
                {
                    Description = "File isn't exist!",
                    OperationStatus = StatusCode.Fail,
                };
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "{ExcetptionType} was catched", typeof(Exception));
                return new Response
                {
                    Description = "Something go wrong",
                    OperationStatus = StatusCode.Fail,
                };
            }
        }

        public IResponse Save(IEnumerable<PHPDemon> savedFiles)
        {
            try
            {
                _logger.LogInformation("Saving {savedFilesCount} files in storage file started", savedFiles?.ToList().Count);
                if (!_repository.SaveAll(savedFiles))
                {
                    _logger.LogError("Storage .json data file not founded");
                    return new DataResponse<PHPDemon>
                    {
                        Description = "Storage json data file not founded",
                        OperationStatus = StatusCode.Fail,
                    };
                }

                _logger.LogInformation("{savedFilesCount} files were succsessfully saved", savedFiles.ToList().Count);
                return new Response
                {
                    Description = "Files were succsessfully saved",
                    OperationStatus = StatusCode.Success,
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{ExcetptionType} was catched", typeof(Exception));
                return new DataResponse<PHPDemon>
                {
                    Description = "Something go wrong",
                    OperationStatus = StatusCode.Fail,
                };
            }
        }
    }
}
