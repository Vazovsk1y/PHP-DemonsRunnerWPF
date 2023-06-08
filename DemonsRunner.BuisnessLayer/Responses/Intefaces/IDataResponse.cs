namespace DemonsRunner.BuisnessLayer.Responses.Interfaces
{
    /// <summary>
    /// Response from services with data, extend base response.
    /// </summary>
    public interface IDataResponse<T> : IResponse
    {
        // represents response from buisness layer that might contain data.

        public T? Data { get; set; }
    }
}
