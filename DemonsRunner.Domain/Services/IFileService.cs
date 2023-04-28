using DemonsRunner.Domain.Interfaces;
using DemonsRunner.Domain.Models;
using DemonsRunner.Domain.Responses;

namespace DemonsRunner.Domain.Services
{
    public interface IFileService
    {
        public IDataResponse<PHPDemon> GetSaved();

        public IBaseResponse Save(IEnumerable<PHPDemon> savedFiles);
    }
}
