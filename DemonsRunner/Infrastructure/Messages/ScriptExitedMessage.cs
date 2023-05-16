using DemonsRunner.ViewModels;

namespace DemonsRunner.Infrastructure.Messages
{
    internal enum ExitType
    {
        ByTaskManager,
        InsideApp
    }

    internal record ScriptExitedMessage(IScriptExecutorViewModel Sender, ExitType ExitType);
}
