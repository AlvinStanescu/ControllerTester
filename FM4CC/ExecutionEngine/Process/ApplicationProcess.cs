using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FM4CC.ExecutionEngine.Process
{
    public abstract class ApplicationProcess
    {
        public abstract void StartProcess();
        public abstract void EndProcess();
    }
}
