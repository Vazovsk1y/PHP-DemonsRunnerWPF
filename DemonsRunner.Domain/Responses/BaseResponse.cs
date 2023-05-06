using DemonsRunner.Domain.Enums;
using DemonsRunner.Domain.Responses.Intefaces;

namespace DemonsRunner.Domain.Responses
{
    public class BaseResponse : IBaseResponse
    {
        public string Description { get; set; }

        public StatusCode OperationStatus { get; set; }
    }
}
