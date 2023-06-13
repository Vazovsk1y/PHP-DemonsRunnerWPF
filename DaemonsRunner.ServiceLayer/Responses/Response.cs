using DaemonsRunner.BuisnessLayer.Responses.Enums;
using DaemonsRunner.BuisnessLayer.Responses.Interfaces;

namespace DaemonsRunner.Domain.Responses
{
    public class Response : IResponse
    {
        public string Description { get; set; }

        public StatusCode OperationStatus { get; set; }
    }
}
