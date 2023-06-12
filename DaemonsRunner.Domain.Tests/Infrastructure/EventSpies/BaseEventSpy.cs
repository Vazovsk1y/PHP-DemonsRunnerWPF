using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaemonsRunner.Domain.Tests.Infrastructure.EventSpies
{
    internal abstract class BaseEventSpy
    {
        public bool EventHandled { get; protected set; }

        public int EventWaitTimeMs => 100;
    }
}
