using DemonsRunner.BuisnessLayer.Responses.Enums;
using DemonsRunner.BuisnessLayer.Responses.Interfaces;

namespace DemonsRunner.Domain.Responses
{
    public class Response : IResponse
    {
        public string Description { get; set; }

        public StatusCode OperationStatus { get; set; }
    }
}
