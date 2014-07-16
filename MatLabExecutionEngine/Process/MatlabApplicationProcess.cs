using FM4CC.ExecutionEngine.Process;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace FM4CC.ExecutionEngine.Matlab.Process
{
    public class MatlabApplicationProcess : ApplicationProcess
    {
        public Object MatLabInstance
        {
            get;
            private set;
        }

        public MatlabApplicationProcess()
        {

        }

        public override void StartProcess()
        {
            if (MatLabInstance == null)
            {
                MatLabInstance = Activator.CreateInstance(Type.GetTypeFromProgID("MATLAB.Application"));
            }
        }

        public override void EndProcess()
        {
            if (MatLabInstance != null)
            {
                try
                {
                    MatLabInstance.GetType().InvokeMember("Quit", BindingFlags.InvokeMethod, null, MatLabInstance, null);
                    System.Diagnostics.Process.GetProcessesByName("MATLAB");
                }
                catch (COMException)
                {
                }
                catch (TargetException)
                {
                }
            }
            MatLabInstance = null;
        }


        public void Dispose()
        {
            EndProcess();
        }

        public override void Kill()
        {
            if (MatLabInstance != null)
            {
                MatLabInstance = null;
                System.Diagnostics.Process[] matlabProcesses = System.Diagnostics.Process.GetProcessesByName("MATLAB");
                foreach (System.Diagnostics.Process matlabProcess in matlabProcesses)
                {
                    matlabProcess.Kill();
                }

            }
        }
    }
}
