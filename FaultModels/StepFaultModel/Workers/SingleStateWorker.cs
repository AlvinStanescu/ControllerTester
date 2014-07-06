using FM4CC.ExecutionEngine;
using FM4CC.FaultModels.Step.Parsers;
using FM4CC.TestCase;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace FM4CC.FaultModels.Step
{
    internal class WorstCaseScenarioWorker : BackgroundWorker
    {
        internal IList<DataGridHeatPoint> SelectedRegions {get; set;}
        internal IList<TestCase.FaultModelTesterTestCase> TestCases { get; set; }

        internal WorstCaseScenarioWorker()
        {
            this.WorkerReportsProgress = true;
            this.DoWork += singleStateWorker_DoWork;
        }

        private void singleStateWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            if (SelectedRegions == null) throw new FM4CCException("Search regions not set");

            StepFaultModel fm = e.Argument as StepFaultModel;
            ExecutionInstance currentTestProject = fm.ExecutionInstance;

            string message = null;
            fm.ExecutionEngine.AcquireProcess();
                
            // Sets up the environment of the execution engine
            fm.ExecutionInstance = currentTestProject;
            fm.SetUpEnvironment();

            int i = 0;
            foreach (DataGridHeatPoint region in SelectedRegions)
            {
                int requirementIndex = 1;

                switch(region.Requirement)
                {
                    case "Stability": 
                        requirementIndex = 1;
                        break;
                    case "Liveness":
                        requirementIndex = 2;
                        break;
                    case "Smoothness":
                        requirementIndex = 3;
                        break;
                    case "Responsiveness":
                        requirementIndex = 4;
                        break;
                    case "Oscillation":
                        requirementIndex = 5;
                        break;
                }

                fm.SetSearchParameters(requirementIndex, region.InitialDesiredRegion, region.FinalDesiredRegion, region.ContainedHeatPoint);

                message = (string)fm.Run("WorstCaseSearch");

                if (!message.ToLower().Contains("success"))
                {
                    e.Result = false;
                    throw new FM4CCException(message);
                }
                i++;
                this.ReportProgress((int)((double)i / SelectedRegions.Count * 100.0));

                string worstPointFile = Path.GetDirectoryName(fm.ExecutionInstance.GetValue("SUTPath")) + "\\Temp\\SingleStateSearch\\SingleStateSearch_WorstCase.csv";
                ProcessWorstCaseResults(region, worstPointFile, fm.ToString());
            }

            // Tears down the environment
            fm.TearDownEnvironment(false);

            // Relinquishes control of the execution engine
            fm.ExecutionEngine.RelinquishProcess();
            e.Result = true;

        }

        private void ProcessWorstCaseResults(DataGridHeatPoint region, string worstPointFile, string faultModelName)
        {
            FaultModelTesterTestCase testCase = new StepFaultModelTestCase();
            testCase.FaultModel = faultModelName;
            testCase.Name = "Worst " + region.Requirement + " in region (" +
                String.Format("{0:0.##}", region.InitialDesiredRegion * region.BaseUnit) + "," +
                String.Format("{0:0.##}", (region.InitialDesiredRegion + 1) * region.BaseUnit) + ")x(" +
                String.Format("{0:0.##}", region.FinalDesiredRegion * region.BaseUnit) + "," +
                String.Format("{0:0.##}", (region.FinalDesiredRegion + 1) * region.BaseUnit) + ")";
            testCase.Input = SingleStateSearchParser.Parse(worstPointFile);
            TestCases.Add(testCase);
        }
    }
}
