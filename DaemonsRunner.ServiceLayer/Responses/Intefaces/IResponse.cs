using DaemonsRunner.BuisnessLayer.Responses.Enums;

namespace DaemonsRunner.BuisnessLayer.Responses.Interfaces
{
    /// <summary>
    /// Base response from services.
    /// </summary>
    public interface IResponse
    {
        // represents the answer that is formed within the business logic.

        /// <summary>
        /// Operation description.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Operation result code.
        /// </summary>
        public StatusCode OperationStatus { get; set; }
    }
}
