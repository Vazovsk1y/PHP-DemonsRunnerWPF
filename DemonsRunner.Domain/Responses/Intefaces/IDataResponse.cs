namespace DemonsRunner.Domain.Responses.Intefaces
{
    /// <summary>
    /// Response from services with data, extend base response.
    /// </summary>
    public interface IDataResponse<T> : IResponse
    {
        public T Data { get; set; }
    }
}
