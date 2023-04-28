using DemonsRunner.Domain.Responses;

namespace DemonsRunner.Domain.Interfaces
{
    public interface IDataResponse<T> : IBaseResponse
    {
        public IEnumerable<T> Data { get; set; }
    }
}
