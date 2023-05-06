using DemonsRunner.Domain.Enums;
using DemonsRunner.Domain.Responses.Intefaces;

namespace DemonsRunner.Domain.Responses
{
    public class CollectionDataResponse<T> : ICollectionDataResponse<T>
    {
        public IEnumerable<T> Data { get; set; }

        public string Description { get; set; }

        public StatusCode OperationStatus { get; set; }
    }
}
