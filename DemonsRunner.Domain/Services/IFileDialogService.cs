using DemonsRunner.Domain.Models;
using DemonsRunner.Domain.Responses;

namespace DemonsRunner.Domain.Interfaces
{
    public interface IFileDialogService
    {
        public Task<ICollectionDataResponse<PHPDemon>> StartDialog();
    }
}
