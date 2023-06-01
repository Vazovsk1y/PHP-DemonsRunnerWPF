namespace DemonsRunner.BuisnessLayer.Services.Interfaces
{
    /// <summary>
    /// Data bus, for interacting view models with each other without having to have each other in dependencies.
    /// If DI using you must register it like singleton object.
    /// </summary>
    public interface IDataBus
    {
        IDisposable RegisterHandler<T>(Action<T> handler);

        void Send<T>(T message);
    }
}
