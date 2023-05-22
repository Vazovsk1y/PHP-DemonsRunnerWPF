using DemonsRunner.Domain.Models;
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

        public FileService(
            IFileRepository<PHPDemon> repository, 
            ILogger<FileService> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public IDataResponse<IEnumerable<PHPDemon>> GetSaved()
        {
            var files = _repository.GetAll().ToList();
            var response = new DataResponse<IEnumerable<PHPDemon>>
            {
                Data = files,
                OperationStatus = StatusCode.Success,
            };

            if (files.Count is 0)
            {
                _logger.LogWarning($"Received files count was [0]");
            }

            response.Description = $"[{files.Count}] files were received.";
            return response;
        }

        public IResponse SaveAll(IEnumerable<PHPDemon> saveFiles)
        {
            ArgumentNullException.ThrowIfNull(saveFiles);

            int savеFilesCount = saveFiles.ToList().Count;
            var response = new Response
            {
                OperationStatus = StatusCode.Success,
            };

            _logger.LogInformation("Saving [{savedFilesCount}] files in storage file started", savеFilesCount);
            _repository.SaveAll(saveFiles);
            _logger.LogInformation("[{savedFilesCount}] files were successfully saved", savеFilesCount);

            response.Description = $"[{savеFilesCount}] files were successfully saved.";
            return response;
        }
    }
}
