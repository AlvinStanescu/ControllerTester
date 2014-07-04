using FM4CC.ExecutionEngine;
using FM4CC.FaultModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FM4CC.FaultModels.Step
{
    public class TestRunWorker: BackgroundWorker
    {
        public double InitialDesired { get; set; }
        public double FinalDesired { get; set; }

        public TestRunWorker()
        {
            this.InitialDesired = 0;
            this.FinalDesired = 0;
            this.WorkerReportsProgress = true;
            this.DoWork += testRunWorker_DoWork;
        }

        private void testRunWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            bool result = true;
            StepFaultModel fm = e.Argument as StepFaultModel;

            fm.ExecutionEngine.AcquireProcess();
                
            // Sets up the environment of the execution engine
            fm.SetUpEnvironment();

            fm.SetTestRunParameters(InitialDesired, FinalDesired);
            result = (bool)fm.Run("TestRun");

            // Tears down the environment
            fm.TearDownEnvironment(false);

            // Relinquishes control of the execution engine
            fm.ExecutionEngine.RelinquishProcess();

            if (!result)
            {
                e.Result = false;
                throw new FM4CCException("Failed to run the model");
            }

            this.ReportProgress(100);
            e.Result = true;
        }
    }
}
