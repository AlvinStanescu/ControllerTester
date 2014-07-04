using FM4CC.ExecutionEngine;
using FM4CC.FaultModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace FM4CC.FaultModels.Step
{

    public class RandomExplorationWorker : BackgroundWorker
    {
        private static System.Timers.Timer aTimer;
        private double estimatedDuration;
        private double passedDuration;

        public RandomExplorationWorker()
        {
            this.WorkerReportsProgress = true;
            this.WorkerSupportsCancellation = true;
            this.DoWork += generationWorker_DoWork;
        }

        private void generationWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            StepFaultModel fm = e.Argument as StepFaultModel;
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

            message = (string)fm.Run("RandomExploration");

            // Tears down the environment
            fm.TearDownEnvironment(false);

            // Relinquishes control of the execution engine
            fm.ExecutionEngine.RelinquishProcess();
            aTimer.Enabled = false;

            if (!message.ToLower().Contains("success"))
            {
                e.Result = false;
                throw new FM4CCException(message);
            }

            e.Result = true;

        }

        private void OnTimedEvent(object sender, ElapsedEventArgs e)
        {
            passedDuration += 100.0;

            this.ReportProgress((int)((passedDuration / estimatedDuration) * 100.0));
        }
    }
}
