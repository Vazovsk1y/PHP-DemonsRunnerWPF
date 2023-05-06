using DemonsRunner.Domain.Enums;
using DemonsRunner.Domain.Responses.Intefaces;

namespace DemonsRunner.Domain.Responses
{
    public class Response : IResponse
    {
        public string Description { get; set; }

        public StatusCode OperationStatus { get; set; }
    }
}
