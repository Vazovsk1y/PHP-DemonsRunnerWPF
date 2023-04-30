using DemonsRunner.Domain.Models;
using DemonsRunner.Domain.Responses;

namespace DemonsRunner.Domain.Services
{
    public interface IFileService
    {
        public ICollectionDataResponse<PHPDemon> GetSaved();

        public IBaseResponse Save(IEnumerable<PHPDemon> savedFiles);
    }
}
