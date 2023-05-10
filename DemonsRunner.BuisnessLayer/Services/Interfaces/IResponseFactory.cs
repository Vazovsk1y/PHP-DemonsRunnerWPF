using DemonsRunner.Domain.Enums;
using DemonsRunner.Domain.Responses.Intefaces;

namespace DemonsRunner.BuisnessLayer.Services.Interfaces
{
    public interface IResponseFactory
    {
        public IResponse CreateResponse(StatusCode statusCode, string description);

        public IDataResponse<T> CreateDataResponse<T>(StatusCode statusCode, string description, T? data = null) where T : class;

        public IDataResponse<IEnumerable<T>> CreateDataResponse<T>(StatusCode statusCode, string description, IEnumerable<T> data);
    }
}
