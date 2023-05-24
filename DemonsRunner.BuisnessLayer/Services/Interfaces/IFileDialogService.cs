using DemonsRunner.Domain.Models;
using DemonsRunner.Domain.Responses.Intefaces;

namespace DemonsRunner.BuisnessLayer.Services.Interfaces
{
    /// <summary>
    /// Dialog service for interacting with system file dialog class.
    /// </summary>
    public interface IFileDialogService
    {
        /// <summary>
        /// Start dialog with file system and provides you a list of selected files in this dialog.
        /// </summary>
        public Task<IDataResponse<IEnumerable<PHPDemon>>> StartDialogAsync();
    }
}
