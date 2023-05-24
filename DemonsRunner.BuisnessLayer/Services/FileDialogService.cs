using DemonsRunner.Domain.Enums;
using DemonsRunner.Domain.Models;
using Microsoft.Win32;
using DemonsRunner.Domain.Responses.Intefaces;
using DemonsRunner.BuisnessLayer.Services.Interfaces;
using Microsoft.Extensions.Logging;
using DemonsRunner.Domain.Responses;

namespace DemonsRunner.BuisnessLayer.Services
{
    public class FileDialogService : IFileDialogService
    {
        private readonly ILogger<FileDialogService> _logger;
        private readonly OpenFileDialog _fileDialog = new()
        {
            Multiselect = true,
            Title = "Choose daemons:",
            Filter = $"php files (*{PHPDemon.EXTENSION}) | *{PHPDemon.EXTENSION}",
            RestoreDirectory = true,
        };

        public FileDialogService(ILogger<FileDialogService> logger)
        {
            _logger = logger;
        }

        public Task<IDataResponse<IEnumerable<PHPDemon>>> StartDialogAsync()
        {
            var response = new DataResponse<IEnumerable<PHPDemon>>()
            {
                OperationStatus = StatusCode.Fail,
            };

            _logger.LogInformation("Dialog started");
            var dialogResult = _fileDialog.ShowDialog();
            _logger.LogInformation("Dialog ended with result: [{dialogResult}]", dialogResult);

            if (dialogResult is bool result && result is true)
            {
                var data = GetDemons().ToList();
                response.OperationStatus = StatusCode.Success;
                response.Data = data;
                response.Description = $"[{data.Count}] files were selected!";

                return Task.FromResult<IDataResponse<IEnumerable<PHPDemon>>>(response);
            }

            response.Description = "No files were selected.";
            return Task.FromResult<IDataResponse<IEnumerable<PHPDemon>>>(response);
        }

        private IEnumerable<PHPDemon> GetDemons()
        {
            _logger.LogInformation("Searching demons in selected files started");
            var fullFilesPath = _fileDialog.FileNames;
            var filesName = _fileDialog.SafeFileNames;
            List<PHPDemon> demons = new();

            for (int i = 0; i < fullFilesPath.Length; i++)
            {
                demons.Add(new PHPDemon
                {
                    Name = filesName[i],
                    FullPath = fullFilesPath[i],
                });
            }

            _logger.LogInformation("Demons count in selected files {demonsCount}", demons.Count);
            return demons;
        }
    }
}
