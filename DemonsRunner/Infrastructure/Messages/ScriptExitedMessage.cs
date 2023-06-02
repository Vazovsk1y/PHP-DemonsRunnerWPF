using DemonsRunner.ViewModels.Interfaces;

namespace DemonsRunner.Infrastructure.Messages
{
    internal enum ExitType
    {
        ByTaskManager,
        ByAppInfrastructure
    }

    internal record ScriptExitedMessage(IScriptExecutorViewModel Sender, ExitType ExitType);
}
