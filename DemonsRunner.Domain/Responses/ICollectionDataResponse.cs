namespace DemonsRunner.Domain.Responses
{
    public interface ICollectionDataResponse<T> : IBaseResponse
    {
        public IEnumerable<T> Data { get; set; }
    }
}
