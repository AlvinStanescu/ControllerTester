using FM4CC.ExecutionEngine.Matlab.Process;
using FM4CC.ExecutionEngine.Process;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FM4CC.ExecutionEngine.Matlab
{
    using System.Runtime.InteropServices;
    using System.Threading;
    using MatlabProcessContainer = FM4CC.ExecutionEngine.Process.UniqueProcessContainer<FM4CC.ExecutionEngine.Matlab.Process.MatlabApplicationProcess>;

    public class MatlabExecutionEngine : ExecutionEnvironment, IObserver<bool>
    {
        private MatlabProcessContainer matlabProcess;
        private bool owningProcess;
        private WeakReference<MLApp.MLApp> matlabApp;
        private System.Threading.EventWaitHandle eventWaitHandle;
        private GCHandle rcwHandle;

        public MatlabExecutionEngine()
        {
            matlabApp = null;
            matlabProcess = null;
            owningProcess = false;
            eventWaitHandle = new EventWaitHandle(false, EventResetMode.AutoReset);
        }

        #region ExecutionEngine Methods

        /// <summary>
        /// Acquires control of the process used by the execution engine
        /// </summary>
        public override void AcquireProcess()
        {
            if (matlabProcess == null)
            {
                matlabProcess = MatlabProcessContainer.Instance;
                this.rcwHandle = GCHandle.Alloc(this.matlabProcess, GCHandleType.Normal);

            }

            WaitForOwnership();
//            matlabApp.Visible = 0;
        }

        /// <summary>
        /// Relinquishes control of the process used by the execution engine
        /// </summary>
        public override void RelinquishProcess()
        {
            GiveUpOwnership();
        }

        public override void PutVariable(string name, object data)
        {
            if (owningProcess)
            {
                MLApp.MLApp app;
                matlabApp.TryGetTarget(out app);
                app.PutWorkspaceData(name, "base", data);
            }
            else
            {
                throw new InvalidOperationException("Execution Engine should be acquired first.");
            }

        }

        public override dynamic GetVariable(string name)
        {
            if (owningProcess)
            {
                MLApp.MLApp app;
                matlabApp.TryGetTarget(out app);
                return app.GetVariable(name, "base");
            }
            else
            {
                throw new InvalidOperationException("Execution Engine should be acquired first.");
            }
        }

        public override string ExecuteCommand(string command)
        {
            if (owningProcess)
            {
                MLApp.MLApp app;
                matlabApp.TryGetTarget(out app);
                return app.Execute(command);
            }
            else
            {
                throw new InvalidOperationException("Execution Engine should be acquired first.");
            }

        }

        public override string PlotVariable(string name)
        {
            if (owningProcess)
            {
                MLApp.MLApp app;
                matlabApp.TryGetTarget(out app);
                return app.Execute("plot(" + name + ")");
            }
            else
            {
                throw new InvalidOperationException("Execution Engine should be acquired first.");
            }

        }

        public override string ChangeWorkingFolder(string fullPath)
        {
            if (owningProcess)
            {
                MLApp.MLApp app;
                matlabApp.TryGetTarget(out app);
                return app.Execute("cd('" + fullPath + "')");
            }
            else
            {
                throw new InvalidOperationException("Execution Engine should be acquired first.");
            }
        }

        public override string RunProgram(string[] parameters)
        {
            return null;
            // do a script            
        }

        public override string Name
        {
            get
            {
                return "MATLab";
            }
        }
        public override IList<string> SystemTypes 
        { 
            get 
            {
                IList<string> types = new List<string>();
                types.Add("Continuous Controller");
                types.Add("Discrete Controller");
                types.Add("Hybrid Controller");
                return types;
            }
        }


        #endregion

        #region IObserver<bool> Callback Methods

        public void OnCompleted()
        {
            throw new NotImplementedException();
        }

        public void OnError(Exception error)
        {
            throw new NotImplementedException();
        }

        public void OnNext(bool value)
        {
            if (value == true)
            {
                eventWaitHandle.Set();
            }
        }
        #endregion

        #region Process Ownership

        private void WaitForOwnership()
        {
            if (owningProcess) return;

            if (!matlabProcess.RequestOwnership(this))
            {
                eventWaitHandle.WaitOne();
            }
            try
            {
                matlabApp = new WeakReference<MLApp.MLApp>(matlabProcess.GetProcess(this).MatLabInstance);
            }
            catch (COMException)
            {
                matlabProcess.RequestProcessRestart(this);
                matlabApp = new WeakReference<MLApp.MLApp>(matlabProcess.GetProcess(this).MatLabInstance);
            }
            owningProcess = true;
        }

        private void GiveUpOwnership()
        {
            if (owningProcess)
            {
                matlabProcess.RelinquishOwnership(this);
                matlabApp = null;
                owningProcess = false;
            }
        }
        #endregion

        #region IDisposable methods
        public override void Dispose()
        {
            if (matlabProcess != null)
            {
                matlabProcess.Dispose();
            }
        }

        #endregion
    }
}
