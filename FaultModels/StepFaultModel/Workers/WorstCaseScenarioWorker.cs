using FM4CC.ExecutionEngine;
using FM4CC.FaultModels.Step.Parsers;
using FM4CC.TestCase;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace FM4CC.FaultModels.Step
{
    internal class WorstCaseScenarioWorker : BackgroundWorker
    {
        internal FM4CCException Exception { get; set; }
        internal IList<DataGridHeatPoint> SelectedRegions {get; set;}
        internal IList<TestCase.FaultModelTesterTestCase> TestCases { get; set; }
        internal Action<string> Logger { get; set; }

        private static System.Timers.Timer aTimer;
        private StepFaultModel fm;
        private bool isRunning;

        internal WorstCaseScenarioWorker()
        {
            this.Exception = null;
            this.WorkerReportsProgress = true;
            this.WorkerSupportsCancellation = true;
            this.DoWork += singleStateWorker_DoWork;
        }

        private void singleStateWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                this.Exception = null;
                isRunning = false;
                if (SelectedRegions == null) throw new FM4CCException("Search regions not set");

                aTimer = new System.Timers.Timer(100);
                aTimer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
                aTimer.Enabled = true;
                aTimer.AutoReset = true;

                fm = e.Argument as StepFaultModel;
                ExecutionInstance currentTestProject = fm.ExecutionInstance;

                string message = null;
                fm.ExecutionEngine.AcquireProcess();

                // Sets up the environment of the execution engine
                fm.ExecutionInstance = currentTestProject;
                fm.SetUpEnvironment();

                e.Result = true;

                int i = 0;
                foreach (DataGridHeatPoint region in SelectedRegions)
                {
                    int requirementIndex = 1;

                    switch (region.Requirement)
                    {
                        case "Stability":
                            requirementIndex = 1;
                            break;
                        case "Precision":
                            requirementIndex = 2;
                            break;
                        case "Smoothness":
                            requirementIndex = 3;
                            break;
                        case "Responsiveness":
                            requirementIndex = 4;
                            break;
                        case "Steadiness":
                            requirementIndex = 5;
                            break;
                        case "NormalizedSmoothness":
                            // the normalization is performed in the parser
                            requirementIndex = 3;
                            break;
                    }

                    fm.SetSearchParameters(requirementIndex, region.InitialDesiredRegion, region.FinalDesiredRegion, region.ContainedHeatPoint, (string)fm.FaultModelConfiguration.GetValue("OptimizationAlgorithm"));

                    isRunning = true;

                    message = (string)fm.Run("WorstCaseSearch");
                                        
                    if (!message.ToLower().Contains("success"))
                    {
                        e.Result = false;
                        this.Exception = new FM4CCException(message);
                        break; 
                    }
                    else
                    {
                        Logger("Step Fault Model - Worst case computation successful for " + region.Requirement);
                    }
                    i++;
                    this.ReportProgress((int)((double)i / SelectedRegions.Count * 100.0));

                    string worstPointFile = Path.GetDirectoryName(fm.ExecutionInstance.GetValue("SUTPath")) + "\\ControllerTesterResults\\SingleStateSearch\\SingleStateSearch_WorstCase.csv";

                    if (region.Requirement != "NormalizedSmoothness")
                    {
                        ProcessWorstCaseResults(region, worstPointFile, fm.ToString(), requirementIndex);
                    }
                    else
                    {
                        ProcessWorstCaseResults(region, worstPointFile, fm.ToString(), 6);
                    }
                }

                aTimer.Enabled = false;

                // Tears down the environment
                fm.TearDownEnvironment(false);

                // Relinquishes control of the execution engine
                fm.ExecutionEngine.RelinquishProcess();
            }
            catch (TargetInvocationException)
            {
                e.Result = false;
            }

        }

        private void ProcessWorstCaseResults(DataGridHeatPoint region, string worstPointFile, string faultModelName, int requirementIndex)
        {
            FaultModelTesterTestCase testCase = new StepFaultModelTestCase();
            testCase.FaultModel = faultModelName;
            testCase.Name = "Worst " + region.Requirement + " (step) in region (" +
                String.Format("{0:0.##}", region.InitialDesiredRegion * region.BaseUnit) + " to " +
                String.Format("{0:0.##}", (region.InitialDesiredRegion + 1) * region.BaseUnit) + ")x(" +
                String.Format("{0:0.##}", region.FinalDesiredRegion * region.BaseUnit) + " to " +
                String.Format("{0:0.##}", (region.FinalDesiredRegion + 1) * region.BaseUnit) + ")";
            testCase.Input = SingleStateSearchParser.Parse(worstPointFile, requirementIndex);
            TestCases.Add(testCase);
        }

        private void OnTimedEvent(object sender, ElapsedEventArgs e)
        {
            if (this.CancellationPending)
            {
                if (isRunning)
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
}
