using DemonsRunner.Domain.Tests.Infrastructure;

namespace DemonsRunner.Domain.Tests.Infrastructure.EventSpies
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
