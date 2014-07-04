using FM4CC.ExecutionEngine;
using FM4CC.FaultModels;
using FM4CC.TestCase;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FM4CC.WPFGUI.Workflow.Workers
{
    public class TestCaseExecutionWorker : BackgroundWorker
    {
        FaultModel fm;
        public TestCaseExecutionWorker(FaultModel fm)
            : base()
        {
            this.fm = fm;
            this.WorkerReportsProgress = true;
            this.DoWork += tceWorker_DoWork;
        }

        private void tceWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            FaultModelTesterTestCase testCase = e.Argument as FaultModelTesterTestCase;
            fm.ExecutionEngine.AcquireProcess();
                
            // Sets up the environment of the execution engine
            fm.SetUpEnvironment();

            testCase.Passed = fm.Run(e.Argument as FaultModelTesterTestCase);

            // Tears down the environment
            fm.TearDownEnvironment(false);

            // Relinquishes control of the execution engine
            fm.ExecutionEngine.RelinquishProcess();
        }
    }
}
