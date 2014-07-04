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
        public MLApp.MLApp MatLabInstance
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
                MatLabInstance = new MLApp.MLApp();
            }
        }

        public override void EndProcess()
        {
            if (MatLabInstance != null)
            {
                try
                {
                    MatLabInstance.Quit();
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
    }
}
