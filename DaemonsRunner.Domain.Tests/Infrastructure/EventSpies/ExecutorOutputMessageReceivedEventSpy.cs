using DaemonsRunner.Domain.Tests.Infrastructure;

namespace DaemonsRunner.Domain.Tests.Infrastructure.EventSpies
{
    internal class ExecutorOutputMessageReceivedEventSpy : BaseEventSpy
    {
        public Task HandleEvent(object sender, string message)
        {
            EventHandled = true;
            return Task.CompletedTask;
        }
    }
}
