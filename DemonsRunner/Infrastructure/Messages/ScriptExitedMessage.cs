using DemonsRunner.ViewModels;

namespace DemonsRunner.Infrastructure.Messages
{
    internal enum ExitType
    {
        OutsideApp,
        InsideApp
    }

    internal record ScriptExitedMessage(IScriptExecutorViewModel Sender, ExitType ExitType);
}
