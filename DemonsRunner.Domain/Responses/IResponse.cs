using DemonsRunner.Domain.Enums;

namespace DemonsRunner.Domain.Interfaces
{
    public interface IResponse<T>
    {
        public string Description { get; set; }

        public IEnumerable<T> Data { get; set; }

        public StatusCode OperationStatus { get; set; }
    }
}
