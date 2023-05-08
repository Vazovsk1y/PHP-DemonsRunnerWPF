using DemonsRunner.Domain.Enums;
using DemonsRunner.Domain.Models;
using Microsoft.Win32;
using DemonsRunner.Domain.Responses.Intefaces;
using DemonsRunner.Domain.Responses;
using DemonsRunner.BuisnessLayer.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace DemonsRunner.BuisnessLayer.Services
{
    public class FileDialogService : IFileDialogService
    {
        private readonly ILogger<FileDialogService> _logger;

        private readonly OpenFileDialog _fileDialog = new()
        {
            Multiselect = true,
            Title = "Choose file:",
        };

        public FileDialogService(ILogger<FileDialogService> logger)
        {
            _logger = logger;
        }

        public Task<IDataResponse<IEnumerable<PHPDemon>>> StartDialog()
        {
            try
            {
                _logger.LogInformation("Dialog started");
                var dialogResult = _fileDialog.ShowDialog();
                _logger.LogInformation("Dialog ended with result: {dialogResult}", dialogResult);

                if (dialogResult is bool result && result is true)
                {
                    var data = GetDemons().ToList();
                    return Task.FromResult<IDataResponse<IEnumerable<PHPDemon>>>(new DataResponse<IEnumerable<PHPDemon>>
                    {
                        OperationStatus = StatusCode.Success,
                        Data = data,
                        Description = $"{data.Count} files were selected!",
                    });
                }

                return Task.FromResult<IDataResponse<IEnumerable<PHPDemon>>>(new DataResponse<IEnumerable<PHPDemon>>
                {
                    OperationStatus = StatusCode.Fail,
                    Description = "No files were selected"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{ExcetptionType} was catched", typeof(Exception));
                return Task.FromResult<IDataResponse<IEnumerable<PHPDemon>>>(new DataResponse<IEnumerable<PHPDemon>>
                {
                    Description = "Something go wrong",
                    OperationStatus = StatusCode.Fail
                });
            }
        }

        private IEnumerable<PHPDemon> GetDemons()
        {
            _logger.LogInformation("Searching demons in selected files started");
            var fullFIlesPath = _fileDialog.FileNames;
            var filesName = _fileDialog.SafeFileNames;
            List<PHPDemon> demons = new();

            for (int i = 0; i < fullFIlesPath.Length; i++)
            {
                demons.Add(new PHPDemon
                {
                    Name = filesName[i],
                    FullPath = fullFIlesPath[i],
                });
            }

            _logger.LogInformation("Demons count in selected files {demonsCount}", demons.Count);
            return demons;
        }
    }
}
