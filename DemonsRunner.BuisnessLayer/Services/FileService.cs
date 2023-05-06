using DemonsRunner.Domain.Models;
using DemonsRunner.Domain.Repositories;
using DemonsRunner.Domain.Responses;
using DemonsRunner.Domain.Services;
using DemonsRunner.BuisnessLayer.Responses;
using System.Diagnostics;
using DemonsRunner.Domain.Enums;

namespace DemonsRunner.BuisnessLayer.Services
{
    public class FileService : IFileService
    {
        private readonly IRepository<PHPDemon> _repository;

        public FileService(IRepository<PHPDemon> repository)
        {
            _repository = repository;
        }

        public ICollectionDataResponse<PHPDemon> GetSaved()
        {
            try
            {
                var files = _repository.GetAll().ToList();

                if (files is null || files.Count == 0)
                {
                    return new CollectionDataResponse<PHPDemon>
                    {
                        Description = "Data json file not founded!",
                        OperationStatus = StatusCode.Fail,
                    };
                }

                return new CollectionDataResponse<PHPDemon>
                {
                    Description = "Files were succsessfully given to you!",
                    OperationStatus = StatusCode.Success,
                    Data = files
                };
            }
            catch(Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return new CollectionDataResponse<PHPDemon>
                {
                    Description = "Something go wrong",
                    OperationStatus = StatusCode.Fail,
                };
            }
        }

        public IBaseResponse IsFileExist(PHPDemon file)
        {
            try
            {
                var fileInfo = new FileInfo(file.FullPath);
                if (fileInfo.Exists)
                {
                    return new BaseResponse
                    {
                        Description = "File exist!",
                        OperationStatus = StatusCode.Success,
                    };
                }

                return new BaseResponse
                {
                    Description = "File isn't exist!",
                    OperationStatus = StatusCode.Fail,
                };
            }
            catch(Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return new BaseResponse
                {
                    Description = "Something go wrong",
                    OperationStatus = StatusCode.Fail,
                };
            }
        }

        public IBaseResponse Save(IEnumerable<PHPDemon> savedFiles)
        {
            try
            {
                _repository.Save(savedFiles);

                return new BaseResponse
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
