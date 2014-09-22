using FM4CC.ExecutionEngine;
using FM4CC.FaultModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace FM4CC.FaultModels.Step
{

    internal class RandomExplorationWorker : BackgroundWorker
    {
        internal FM4CCException Exception { get; set; }

        private static System.Timers.Timer aTimer;
        private double estimatedDuration;
        private double passedDuration;
        private StepFaultModel fm;
        private bool isRunning;

        internal RandomExplorationWorker()
        {
            this.Exception = null;
            this.WorkerReportsProgress = true;
            this.WorkerSupportsCancellation = true;
            this.DoWork += generationWorker_DoWork;
        }

        private void generationWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                this.Exception = null;
                isRunning = false;
                fm = e.Argument as StepFaultModel;
                ExecutionInstance currentTestProject = fm.ExecutionInstance;

                string message = null;
                fm.ExecutionEngine.AcquireProcess();

                // Sets up the environment of the execution engine
                fm.ExecutionInstance = currentTestProject;
                fm.SetUpEnvironment();

                passedDuration = 0.0;
                estimatedDuration = fm.GetEstimatedDuration("RandomExploration").TotalMilliseconds;

                aTimer = new System.Timers.Timer(100);
                aTimer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
                aTimer.Enabled = true;
                aTimer.AutoReset = true;

                isRunning = true;
                message = (string)fm.Run("RandomExploration");

                // Tears down the environment
                fm.TearDownEnvironment(false);

                // Relinquishes control of the execution engine
                fm.ExecutionEngine.RelinquishProcess();
                aTimer.Enabled = false;

                if (message.ToLower().Contains("success"))
                {
                    e.Result = true;
                }
                else
                {
                    e.Result = false;
                    this.Exception = new FM4CCException(message);
                }
            }
            catch(TargetInvocationException)
            {
                e.Result = false;
            }
        }

        private void OnTimedEvent(object sender, ElapsedEventArgs e)
        {
            passedDuration += 100.0;

            this.ReportProgress((int)((passedDuration / estimatedDuration) * 100.0));
            
            if (this.CancellationPending && isRunning)
            {
                // kill the execution engine and relinquish control
                aTimer.Enabled = false;

                fm.ExecutionEngine.Kill();
                fm.TearDownEnvironment(false);
                fm.ExecutionEngine.RelinquishProcess();
            }
        }
    }
}
