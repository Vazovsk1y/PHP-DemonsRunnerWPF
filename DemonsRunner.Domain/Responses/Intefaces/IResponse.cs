using DemonsRunner.Domain.Enums;

namespace DemonsRunner.Domain.Responses.Intefaces
{
    public interface IResponse
    {
        public string Description { get; set; }

        public StatusCode OperationStatus { get; set; }
    }
}
