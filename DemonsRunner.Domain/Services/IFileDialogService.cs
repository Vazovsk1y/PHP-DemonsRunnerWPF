using DemonsRunner.Models;

namespace DemonsRunner.Interfaces
{
    public interface IFileDialogService
    {
        public IResponse<PHPDemon> StartDialog();
    }
}
