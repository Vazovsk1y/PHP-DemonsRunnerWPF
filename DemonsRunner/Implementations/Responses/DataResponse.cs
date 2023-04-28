using DemonsRunner.Domain.Enums;
using DemonsRunner.Domain.Interfaces;
using System.Collections.Generic;

namespace DemonsRunner.Implementations.Responses
{
    public class DataResponse<T> : IDataResponse<T>
    {
        public IEnumerable<T> Data { get; set; }

        public string Description { get; set; }

        public StatusCode OperationStatus { get; set; }
    }
}
