using DemonsRunner.DAL.Repositories;
using DemonsRunner.Domain.Interfaces;
using DemonsRunner.Domain.Models;
using DemonsRunner.Domain.Repositories;
using DemonsRunner.Domain.Responses;
using DemonsRunner.Domain.Services;
using DemonsRunner.Implementations.Responses;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace DemonsRunner.Implementations.Services
{
    internal class FileService : IFileService
    {
        private readonly IFileRepository<PHPDemon> _repository;

        public FileService(IFileRepository<PHPDemon> repository)
        {
            _repository = repository;
        }

        public IDataResponse<PHPDemon> GetSaved()
        {
            try
            {
                var files = _repository.GetAllFiles().ToList();

                if (files is null || files.Count == 0)
                {
                    return new DataResponse<PHPDemon>
                    {
                        Description = "Data json file not founded!",
                        OperationStatus = Domain.Enums.StatusCode.Fail,
                    };
                }

                return new DataResponse<PHPDemon>
                {
                    Description = "Files were succsessfully given to you!",
                    OperationStatus = Domain.Enums.StatusCode.Success,
                    Data = files
                };
            }
            catch(Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return new DataResponse<PHPDemon>
                {
                    Description = "Something go wrong",
                    OperationStatus = Domain.Enums.StatusCode.Fail,
                };
            }
        }

        public IBaseResponse Save(IEnumerable<PHPDemon> savedFiles)
        {
            try
            {
                _repository.SaveFiles(savedFiles);

                return new BaseResponse
                {
                    Description = "Files were succsessfully saved",
                    OperationStatus = Domain.Enums.StatusCode.Success,
                };
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return new DataResponse<PHPDemon>
                {
                    Description = "Something go wrong",
                    OperationStatus = Domain.Enums.StatusCode.Fail,
                };
            }
        }
    }
}
