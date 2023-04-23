using DemonsRunner.Domain.Models;

namespace DemonsRunner.Domain.Interfaces
{
    public interface IFileDialogService
    {
        public IResponse<PHPDemon> StartDialog();
    }
}
