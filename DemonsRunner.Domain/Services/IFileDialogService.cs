using DemonsRunner.Domain.Models;

namespace DemonsRunner.Domain.Interfaces
{
    public interface IFileDialogService
    {
        public IDataResponse<PHPDemon> StartDialog();
    }
}
