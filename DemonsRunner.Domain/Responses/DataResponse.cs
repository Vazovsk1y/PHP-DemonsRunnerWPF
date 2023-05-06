using DemonsRunner.Domain.Enums;
using DemonsRunner.Domain.Responses.Intefaces;

namespace DemonsRunner.Domain.Responses
{
    public class DataResponse<T> : IDataResponse<T>
    {
        public T Data { get; set; }

        public string Description { get; set; }

        public StatusCode OperationStatus { get; set; }
    }
}
