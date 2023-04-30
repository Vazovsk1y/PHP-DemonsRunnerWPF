using DemonsRunner.Domain.Responses;

namespace DemonsRunner.Domain.Interfaces
{
    public interface IDataResponse<T> : IBaseResponse
    {
        public T Data { get; set; }
    }
}
