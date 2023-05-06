namespace DemonsRunner.Domain.Responses.Intefaces
{
    public interface IDataResponse<T> : IResponse
    {
        public T Data { get; set; }
    }
}
