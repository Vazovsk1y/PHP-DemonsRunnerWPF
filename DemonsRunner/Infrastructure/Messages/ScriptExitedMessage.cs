using DemonsRunner.ViewModels;
using System;

namespace DemonsRunner.Infrastructure.Messages
{
    internal record ScriptExitedMessage(IScriptExecutorViewModel Sender, EventArgs EventArgs);
}
