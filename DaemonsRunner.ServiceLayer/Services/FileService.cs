using DaemonsRunner.Domain.Models;
using DaemonsRunner.DAL.Repositories.Interfaces;
using DaemonsRunner.Domain.Responses;
using DaemonsRunner.BuisnessLayer.Services.Interfaces;
using Microsoft.Extensions.Logging;
using DaemonsRunner.BuisnessLayer.Responses.Enums;
using DaemonsRunner.BuisnessLayer.Responses.Interfaces;
using DaemonsRunner.BuisnessLayer.Responses;

namespace DaemonsRunner.BuisnessLayer.Services
{
    public class FileService : IFileService
    {
        private readonly IFileRepository<PHPFile> _repository;
        private readonly ILogger<FileService> _logger;

        public FileService(
            IFileRepository<PHPFile> repository, 
            ILogger<FileService> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public IDataResponse<IEnumerable<PHPFile>> GetSaved()
        {
            var files = _repository.GetAll().ToList();
            var response = new DataResponse<IEnumerable<PHPFile>>
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

        public IResponse SaveAll(IEnumerable<PHPFile> saveFiles)
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
