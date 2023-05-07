using System.Diagnostics;
using DemonsRunner.Domain.Enums;
using DemonsRunner.Domain.Models;
using Microsoft.Win32;
using DemonsRunner.Domain.Responses.Intefaces;
using DemonsRunner.Domain.Responses;
using DemonsRunner.BuisnessLayer.Services.Interfaces;

namespace DemonsRunner.BuisnessLayer.Services
{
    public class FileDialogService : IFileDialogService
    {
        private readonly OpenFileDialog _fileDialog = new()
        {
            Multiselect = true,
            Title = "Выберите файл:",
        };

        public Task<IDataResponse<IEnumerable<PHPDemon>>> StartDialog()
        {
            try
            {
                var dialogResult = _fileDialog.ShowDialog();

                return dialogResult is bool result && !result ?
                    Task.FromResult<IDataResponse<IEnumerable<PHPDemon>>>(new DataResponse<IEnumerable<PHPDemon>>
                    {
                        OperationStatus = StatusCode.Fail
                    })
                    :
                    Task.FromResult<IDataResponse<IEnumerable<PHPDemon>>>(new DataResponse<IEnumerable<PHPDemon>>
                    {
                        OperationStatus = StatusCode.Success,
                        Data = GetDemons()
                    });
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return Task.FromResult<IDataResponse<IEnumerable<PHPDemon>>>(new DataResponse<IEnumerable<PHPDemon>>
                {
                    Description = "Something go wrong",
                    OperationStatus = StatusCode.Fail
                });
            }
        }

        private IEnumerable<PHPDemon> GetDemons()
        {
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

            return demons;
        }
    }
}
