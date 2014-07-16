using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FM4CC.ExecutionEngine
{
    public abstract class ExecutionEnvironment : IDisposable
    {
        public abstract void AcquireProcess();
        public abstract void RelinquishProcess();
        public abstract void PutVariable(string name, object data);
        public abstract dynamic GetVariable(string name);
        public abstract string ExecuteCommand(string command);
        public abstract string PlotVariable(string name);
        public abstract string ChangeWorkingFolder(string fullPath);
        public abstract string RunProgram(string[] parameters);

        public abstract string Name {get;}
        public abstract IList<string> SystemTypes {get; }
        public abstract void Dispose();
        public abstract void Kill();
    }
}
