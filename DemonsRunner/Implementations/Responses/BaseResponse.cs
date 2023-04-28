using DemonsRunner.Domain.Enums;
using DemonsRunner.Domain.Responses;

namespace DemonsRunner.Implementations.Responses
{
    public class BaseResponse : IBaseResponse
    {
        public string Description { get; set; }

        public StatusCode OperationStatus { get; set; }
    }
}
