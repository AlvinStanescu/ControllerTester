using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FM4CC.ExecutionEngine.Process
{
    public abstract class ApplicationProcess
    {
        public abstract void StartProcess(string path);
        public abstract void EndProcess();
        public abstract void Kill();
    }
}
