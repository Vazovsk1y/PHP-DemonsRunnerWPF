using DemonsRunner.Domain.Models;
using DemonsRunner.Domain.Responses.Intefaces;

namespace DemonsRunner.BuisnessLayer.Services.Interfaces
{
    public interface IFileService
    {
        public ICollectionDataResponse<PHPDemon> GetSaved();

        public IBaseResponse Save(IEnumerable<PHPDemon> savedFiles);

        public IBaseResponse IsFileExist(PHPDemon file);
    }
}
