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
        public static string Path { get; set; }

        private MatlabProcessContainer matlabProcess;
        private bool owningProcess;
        private WeakReference<Object> matlabApp;
        private System.Threading.EventWaitHandle eventWaitHandle;
        private GCHandle rcwHandle;

        public MatlabExecutionEngine(string path)
        {
            Path = path;
            matlabApp = null;
            matlabProcess = null;
            owningProcess = false;
            eventWaitHandle = new EventWaitHandle(false, EventResetMode.AutoReset);
        }

        private void RestartProcess()
        {
            matlabProcess.RequestProcessRestart(this);
            matlabApp = new WeakReference<object>(matlabProcess.GetProcess(this).MatLabInstance);
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
                object app;
                matlabApp.TryGetTarget(out app);

                if (app == null)
                {
                    RestartProcess();
                    matlabApp.TryGetTarget(out app);
                }

                app.GetType().InvokeMember("PutWorkspaceData", System.Reflection.BindingFlags.InvokeMethod, null, app, new object[] { name, "base", data });                    
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
                object app;
                matlabApp.TryGetTarget(out app);

                if (app == null)
                {
                    RestartProcess();
                    matlabApp.TryGetTarget(out app);
                }

                return app.GetType().InvokeMember("GetVariable", System.Reflection.BindingFlags.InvokeMethod, null, app, new object[] { name, "base" });
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
                object app;
                matlabApp.TryGetTarget(out app);
                
                if (app == null)
                {
                    RestartProcess();
                    matlabApp.TryGetTarget(out app);
                }

                return (string)app.GetType().InvokeMember("Execute", System.Reflection.BindingFlags.InvokeMethod, null, app, new object[] { command });
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
                object app;
                matlabApp.TryGetTarget(out app);

                if (app == null)
                {
                    RestartProcess();
                    matlabApp.TryGetTarget(out app);
                }

                return (string)app.GetType().InvokeMember("Execute", System.Reflection.BindingFlags.InvokeMethod, null, app, new object[] { "plot(" + name + ")" });
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
                object app;
                matlabApp.TryGetTarget(out app);

                if (app == null)
                {
                    RestartProcess();
                    matlabApp.TryGetTarget(out app);
                }

                return (string)app.GetType().InvokeMember("Execute", System.Reflection.BindingFlags.InvokeMethod, null, app, new object[] { "cd('" + fullPath + "')" });
            }
            else
            {
                throw new InvalidOperationException("Execution Engine should be acquired first.");
            }
        }

        public override string RunProgram(string[] parameters)
        {
            // TODO
            return null;
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
        public override void Kill()
        {
            if (matlabProcess != null)
            {
                matlabProcess.Kill();
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
                matlabApp = new WeakReference<object>(matlabProcess.GetProcess(this).MatLabInstance);
            }
            catch (COMException)
            {
                RestartProcess();
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
