using DaemonsRunner.BuisnessLayer.Responses.Interfaces;
using DaemonsRunner.Domain.Models;

namespace DaemonsRunner.BuisnessLayer.Services.Interfaces
{
    /// <summary>
    /// Dialog service for interacting with system class OpenFileDialog.
    /// </summary>
    public interface IFileDialog
    {
        /// <summary>
        /// Start dialog with file system and provides you a response that contain all selected files in this dialog.
        /// </summary>
        /// <returns>
        /// IDataResponse that might contain data, if operation status is success.
        /// </returns>
        public Task<IDataResponse<IEnumerable<PHPFile>>> StartDialogAsync();
    }
}
