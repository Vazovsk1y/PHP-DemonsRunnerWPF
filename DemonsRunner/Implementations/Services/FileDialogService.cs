using System;
using System.Collections.Generic;
using System.Diagnostics;
using DemonsRunner.Enums;
using DemonsRunner.Implementations.Responses;
using DemonsRunner.Interfaces;
using DemonsRunner.Models;
using Microsoft.Win32;

namespace DemonsRunner.Implementations.Services
{
    internal class FileDialogService : IFileDialogService
    {
        private readonly OpenFileDialog fileDialog = new()
        {
            Multiselect = true,
        };

        public IResponse<PHPDemon> StartDialog()
        {
            try
            {
                var dialogResult = fileDialog.ShowDialog();

                return dialogResult is bool result && !result ?
                    new Response<PHPDemon>
                    {
                        OperationStatus = StatusCode.Fail
                    }
                    :
                    new Response<PHPDemon>
                    {
                        OperationStatus = StatusCode.Success,
                        Data = GetDemons()
                    };
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return new Response<PHPDemon>
                {
                    OperationStatus = StatusCode.Fail
                };
            }

        }

        private IEnumerable<PHPDemon> GetDemons()
        {
            var fullFIlesPath = fileDialog.FileNames;
            var filesName = fileDialog.SafeFileNames;
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
