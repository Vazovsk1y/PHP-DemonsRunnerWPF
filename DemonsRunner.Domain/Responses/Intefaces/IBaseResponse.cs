using DemonsRunner.Domain.Enums;

namespace DemonsRunner.Domain.Responses.Intefaces
{
    public interface IBaseResponse
    {
        public string Description { get; set; }

        public StatusCode OperationStatus { get; set; }
    }
}
