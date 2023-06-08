using DemonsRunner.BuisnessLayer.Responses.Enums;
using DemonsRunner.BuisnessLayer.Responses.Interfaces;

namespace DemonsRunner.BuisnessLayer.Responses
{
    public class DataResponse<T> : IDataResponse<T>
    {
        public T? Data { get; set; }

        public string Description { get; set; }

        public StatusCode OperationStatus { get; set; }
    }
}
