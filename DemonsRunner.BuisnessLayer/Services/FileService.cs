using DemonsRunner.Domain.Models;
using System.Diagnostics;
using DemonsRunner.Domain.Enums;
using DemonsRunner.Domain.Responses.Intefaces;
using DemonsRunner.DAL.Repositories.Interfaces;
using DemonsRunner.Domain.Responses;
using DemonsRunner.BuisnessLayer.Services.Interfaces;

namespace DemonsRunner.BuisnessLayer.Services
{
    public class FileService : IFileService
    {
        private readonly IFileRepository<PHPDemon> _repository;

        public FileService(IFileRepository<PHPDemon> repository)
        {
            _repository = repository;
        }

        public IDataResponse<IEnumerable<PHPDemon>> GetSaved()
        {
            try
            {
                var files = _repository.GetAll().ToList();

                if (files is null || files.Count == 0)
                {
                    return new DataResponse<IEnumerable<PHPDemon>>
                    {
                        Description = "Data json file not founded!",
                        OperationStatus = StatusCode.Fail,
                    };
                }

                return new DataResponse<IEnumerable<PHPDemon>>
                {
                    Description = "Files were succsessfully given to you!",
                    OperationStatus = StatusCode.Success,
                    Data = files
                };
            }
            catch(Exception ex)
            {
                Debug.WriteLine(ex.Message);
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
                var fileInfo = new FileInfo(file.FullPath);
                if (fileInfo.Exists)
                {
                    return new Response
                    {
                        Description = "File exist!",
                        OperationStatus = StatusCode.Success,
                    };
                }

                return new Response
                {
                    Description = "File isn't exist!",
                    OperationStatus = StatusCode.Fail,
                };
            }
            catch(Exception ex)
            {
                Debug.WriteLine(ex.Message);
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
                _repository.SaveAll(savedFiles);

                return new Response
                {
                    Description = "Files were succsessfully saved",
                    OperationStatus = StatusCode.Success,
                };
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return new DataResponse<PHPDemon>
                {
                    Description = "Something go wrong",
                    OperationStatus = StatusCode.Fail,
                };
            }
        }
    }
}
