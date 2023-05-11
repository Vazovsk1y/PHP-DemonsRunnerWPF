using DemonsRunner.Domain.Enums;
using DemonsRunner.Domain.Models;
using Microsoft.Win32;
using DemonsRunner.Domain.Responses.Intefaces;
using DemonsRunner.BuisnessLayer.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace DemonsRunner.BuisnessLayer.Services
{
    public class FileDialogService : IFileDialogService
    {
        private readonly ILogger<FileDialogService> _logger;
        private readonly IResponseFactory _responseFactory;

        private readonly OpenFileDialog _fileDialog = new()
        {
            Multiselect = true,
            Title = "Choose file:",
        };

        public FileDialogService(ILogger<FileDialogService> logger, IResponseFactory responseFactory)
        {
            _logger = logger;
            _responseFactory = responseFactory;
        }

        public Task<IDataResponse<IEnumerable<PHPDemon>>> StartDialog()
        {
            string messageResponse = string.Empty;
            try
            {
                _logger.LogInformation("Dialog started");
                var dialogResult = _fileDialog.ShowDialog();
                _logger.LogInformation("Dialog ended with result: [{dialogResult}]", dialogResult);

                if (dialogResult is bool result && result is true)
                {
                    var data = GetDemons();
                    messageResponse = $"[{data.ToList().Count}] files were selected!";

                    return Task.FromResult(_responseFactory.CreateDataResponse(StatusCode.Success, messageResponse , data));
                }

                messageResponse = "No files were selected";
                return Task.FromResult(_responseFactory.CreateDataResponse(StatusCode.Fail, messageResponse, Enumerable.Empty<PHPDemon>()));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{ExcetptionType} was catched", typeof(Exception));
                return Task.FromResult(_responseFactory.CreateDataResponse(StatusCode.Fail, "Something go wrong", Enumerable.Empty<PHPDemon>()));
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
