using FM4CC.ExecutionEngine;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace FM4CC.FaultModels.Step
{
    internal class WorstCaseScenarioWorker : BackgroundWorker
    {
        internal IList<DataGridHeatPoint> SelectedRegions {get; set;}

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
            }

            // Tears down the environment
            fm.TearDownEnvironment(false);

            // Relinquishes control of the execution engine
            fm.ExecutionEngine.RelinquishProcess();
            e.Result = true;

        }

    }
}
