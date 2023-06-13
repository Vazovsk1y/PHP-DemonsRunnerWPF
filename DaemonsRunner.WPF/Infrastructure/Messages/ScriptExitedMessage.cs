using DaemonsRunner.ViewModels.Interfaces;

namespace DaemonsRunner.Infrastructure.Messages
{
    internal enum ExitType
    {
        ByTaskManager,
        ByAppInfrastructure
    }

    internal record ScriptExitedMessage(IScriptExecutorViewModel Sender, ExitType ExitType);
}
