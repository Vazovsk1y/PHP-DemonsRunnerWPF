using DemonsRunner.BuisnessLayer.Services.Interfaces;
using DemonsRunner.Domain.Enums;
using DemonsRunner.Domain.Responses;
using DemonsRunner.Domain.Responses.Intefaces;

namespace DemonsRunner.BuisnessLayer.Services
{
    public class ResponseFactory : IResponseFactory
    {
        public IResponse CreateResponse(StatusCode statusCode, string description)
        {
            return new Response
            {
                Description = description,
                OperationStatus = statusCode,
            };
        }

        public IDataResponse<T> CreateDataResponse<T>(StatusCode statusCode, string description, T? data = null) where T : class
        {
            return new DataResponse<T>
            {
                OperationStatus = statusCode,
                Description = description,
                Data = data
            };
        }

        public IDataResponse<IEnumerable<T>> CreateDataResponse<T>(StatusCode statusCode, string description, IEnumerable<T> data)
        {
            return new DataResponse<IEnumerable<T>>
            {
                OperationStatus = statusCode,
                Description = description,
                Data = data
            };
        }
    }
}
