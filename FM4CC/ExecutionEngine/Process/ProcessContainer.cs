using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FM4CC.ExecutionEngine.Process
{
    public interface ProcessContainer<T> where T : ApplicationProcess
    {
        bool RequestOwnership(IObserver<bool> newOwner);
        T GetProcess(IObserver<bool> owner);
        void RelinquishOwnership(IObserver<bool> currentOwner);
        void CancelWaiting(IObserver<bool> waitingObject);
        void RequestProcessRestart(IObserver<bool> owner);

    }
}
