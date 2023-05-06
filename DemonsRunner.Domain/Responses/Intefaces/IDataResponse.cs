namespace DemonsRunner.Domain.Responses.Intefaces
{
    public interface IDataResponse<T> : IBaseResponse
    {
        public T Data { get; set; }
    }
}
