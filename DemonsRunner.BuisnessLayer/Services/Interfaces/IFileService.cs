using DemonsRunner.Domain.Models;
using DemonsRunner.Domain.Responses.Intefaces;

namespace DemonsRunner.BuisnessLayer.Services.Interfaces
{
    public interface IFileService
    {
        public IDataResponse<IEnumerable<PHPDemon>> GetSaved();

        public IResponse Save(IEnumerable<PHPDemon> savedFiles);

        public IResponse IsFileExist(PHPDemon file);
    }
}
