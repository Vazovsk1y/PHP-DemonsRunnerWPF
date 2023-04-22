using DemonsRunner.Enums;
using DemonsRunner.Interfaces;
using System.Collections.Generic;

namespace DemonsRunner.Implementations.Responses
{
    public class Response<T> : IResponse<T>
    {
        public string Description { get; set; }
        public IEnumerable<T> Data { get; set; }
        public StatusCode OperationStatus { get; set; }
    }
}
