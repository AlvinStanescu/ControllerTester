using FM4CC.ExecutionEngine.Process;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
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

        public override void StartProcess(string matlabFolderPath)
        {
            if (MatLabInstance == null)
            {
                MatLabInstance = Activator.CreateInstance(Type.GetTypeFromProgID("MATLAB.Application"));
                
                // wait for the MatLabInstance to actually accept input, since, if the accelerated model is not built a-priori, it will get stuck in accelbuild
                Thread.Sleep(15000);

            }
        }

        public override void EndProcess()
        {
            if (MatLabInstance != null)
            {
                try
                {
                    MatLabInstance.GetType().InvokeMember("Execute", System.Reflection.BindingFlags.InvokeMethod, null, MatLabInstance, new object[] { "close_system(gcs,0);" });
                    MatLabInstance.GetType().InvokeMember("Quit", BindingFlags.InvokeMethod, null, MatLabInstance, null);
                }
                catch (COMException)
                {
                }
                catch (TargetException)
                {
                }
                catch(TargetInvocationException)
                {
                    System.Diagnostics.Process[] matlabProcesses = System.Diagnostics.Process.GetProcessesByName("MATLAB");
                    foreach (System.Diagnostics.Process matlabProcess in matlabProcesses)
                    {
                        matlabProcess.Kill();
                    }
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
