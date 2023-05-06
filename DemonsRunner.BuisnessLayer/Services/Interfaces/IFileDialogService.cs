using DemonsRunner.Domain.Models;
using DemonsRunner.Domain.Responses.Intefaces;

namespace DemonsRunner.BuisnessLayer.Services.Interfaces
{
    public interface IFileDialogService
    {
        public Task<IDataResponse<IEnumerable<PHPDemon>>> StartDialog();
    }
}
