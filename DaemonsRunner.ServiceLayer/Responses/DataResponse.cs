using DaemonsRunner.BuisnessLayer.Responses.Enums;
using DaemonsRunner.BuisnessLayer.Responses.Interfaces;

namespace DaemonsRunner.BuisnessLayer.Responses
{
    public class DataResponse<T> : IDataResponse<T>
    {
        public T? Data { get; set; }

        public string Description { get; set; }

        public StatusCode OperationStatus { get; set; }
    }
}
