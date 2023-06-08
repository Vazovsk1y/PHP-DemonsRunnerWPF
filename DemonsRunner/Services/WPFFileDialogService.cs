using DemonsRunner.Domain.Models;
using Microsoft.Win32;
using DemonsRunner.BuisnessLayer.Services.Interfaces;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using DemonsRunner.BuisnessLayer.Responses.Enums;
using DemonsRunner.BuisnessLayer.Responses.Interfaces;
using DemonsRunner.BuisnessLayer.Responses;

namespace DemonsRunner.Services
{
    public class WPFFileDialogService : IFileDialog
    {
        private readonly ILogger<WPFFileDialogService> _logger;
        private readonly OpenFileDialog _fileDialog = new()
        {
            Multiselect = true,
            Title = "Choose files:",
            Filter = "php files (*.php) | *.php",
            RestoreDirectory = true,
        };

        public WPFFileDialogService(ILogger<WPFFileDialogService> logger)
        {
            _logger = logger;
        }

        public Task<IDataResponse<IEnumerable<PHPFile>>> StartDialogAsync()
        {
            var response = new DataResponse<IEnumerable<PHPFile>>()
            {
                OperationStatus = StatusCode.Fail,
            };

            _logger.LogInformation("Dialog started");
            var dialogResult = _fileDialog.ShowDialog();
            _logger.LogInformation("Dialog ended with result: [{dialogResult}]", dialogResult);

            if (dialogResult is bool result && result is true)
            {
                var data = GetPHPFiles().ToList();
                response.OperationStatus = StatusCode.Success;
                response.Data = data;
                response.Description = $"[{data.Count}] files were selected!";

                return Task.FromResult<IDataResponse<IEnumerable<PHPFile>>>(response);
            }

            response.Description = "No files were selected.";
            return Task.FromResult<IDataResponse<IEnumerable<PHPFile>>>(response);
        }

        private IEnumerable<PHPFile> GetPHPFiles()
        {
            _logger.LogInformation("Searching php-files in selected files started");
            var fullFilesPath = _fileDialog.FileNames;
            var filesName = _fileDialog.SafeFileNames;
            List<PHPFile> files = new();

            for (int i = 0; i < fullFilesPath.Length; i++)
            {
                files.Add(new PHPFile(filesName[i], fullFilesPath[i]));
            }

            _logger.LogInformation("Php-files count in selected files [{demonsCount}]", files.Count);
            return files;
        }
    }
}
