using FM4CC.FaultModels;
using FM4CC.ExecutionEngine;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.IO;
using FM4CC.Environment;
using System.ComponentModel;
using FM4CC.Util.Heatmap;
using System.Globalization;
using FM4CC.TestCase;

namespace FM4CC.FaultModels.Step
{
    public class StepFaultModel : FaultModel
    {
        private bool setupDone;
        private const string prefix = "CT_";
        private const string shortName = "StepFaultModel";
        private const string name = "Step Desired Value";
        private const string description = "Two values are generated: an initial desired value that is greater than the actual value of the system at rest, usually 0. Giving the system enough time to stabilize at the initial desired value, a final desired value is generated and input to the control loop, waiting for the stabilization of the system under test at this final desired value.";
        private string scriptsPath;

        #region Fault Model Implementation

        public StepFaultModel(ExecutionEnvironment executionEngine, string scriptsPath)
        {
            setupDone = false;
            this.scriptsPath = scriptsPath;
            this.ExecutionEngine = executionEngine;
            this.FaultModelConfiguration = new StepFaultModelConfiguration();
            this.RandomExplorationWorker = new RandomExplorationWorker();
            this.WorstCaseWorker = new WorstCaseScenarioWorker();
            this.TestRunWorker = new TestRunWorker();
        }

        public StepFaultModel(ExecutionEnvironment executionEngine, FaultModelConfiguration configuration, string scriptsPath)
        {
            setupDone = false;
            this.scriptsPath = scriptsPath;
            this.ExecutionEngine = executionEngine;
            this.FaultModelConfiguration = (StepFaultModelConfiguration)configuration;
            this.RandomExplorationWorker = new RandomExplorationWorker();
            this.WorstCaseWorker = new WorstCaseScenarioWorker();
            this.TestRunWorker = new TestRunWorker();
        }

        public override void SetUpEnvironment()
        {
            if (!setupDone)
            {
                // Get hold of the execution engine if not already
                ExecutionEngine.AcquireProcess();

                // Clear the workspace
                ExecutionEngine.ExecuteCommand("clear all;");

                ExecutionEngine.PutVariable(prefix + "AccelerationDisabled", false);
                ExecutionEngine.PutVariable(prefix + "ScriptsPath", scriptsPath);

                string modelFile = ExecutionInstance.GetValue("SUTPath");
                string modelPath = modelFile.Substring(0, modelFile.LastIndexOf('\\'));

                ExecutionEngine.PutVariable(prefix + "ModelFile", modelFile);
                ExecutionEngine.PutVariable(prefix + "ModelPath", modelPath);
                ExecutionEngine.PutVariable(prefix + "ModelConfigurationFile", ExecutionInstance.GetValue("SUTSettingsPath"));
                ExecutionEngine.PutVariable(prefix + "ModelSimulationTime", SimulationSettings.ModelSimulationTime);

                string tempPath = modelPath + "\\ControllerTesterResults";
                if (!Directory.Exists(tempPath))
                {
                    Directory.CreateDirectory(tempPath);
                }
                ExecutionEngine.PutVariable(prefix + "TempPath", tempPath);
                ExecutionEngine.PutVariable(prefix + "UserTempPath", System.IO.Path.GetTempPath());
                // Add primitive parameters directly to the execution engine
                Dictionary<string, object>.Enumerator primitiveEnumerator = FaultModelConfiguration.GetParametersEnumerator();

                while (primitiveEnumerator.MoveNext())
                {
                    KeyValuePair<string, object> kvPair = primitiveEnumerator.Current;
                    ExecutionEngine.PutVariable(prefix + kvPair.Key, kvPair.Value);
                }

                ExecutionEngine.PutVariable(prefix + "DesiredValueRangeStart", Convert.ToDouble(this.SimulationSettings.DesiredVariable.FromValue));
                ExecutionEngine.PutVariable(prefix + "DesiredValueRangeEnd", Convert.ToDouble(this.SimulationSettings.DesiredVariable.ToValue));
                ExecutionEngine.PutVariable(prefix + "DesiredVariableName", this.SimulationSettings.DesiredVariable.Name);

                ExecutionEngine.PutVariable(prefix + "ActualValueRangeStart", Convert.ToDouble(this.SimulationSettings.ActualVariable.FromValue));
                ExecutionEngine.PutVariable(prefix + "ActualValueRangeEnd", Convert.ToDouble(this.SimulationSettings.ActualVariable.ToValue));
                ExecutionEngine.PutVariable(prefix + "ActualVariableName", this.SimulationSettings.ActualVariable.Name);

                ExecutionEngine.PutVariable(prefix + "TimeStable", this.SimulationSettings.StableStartTime);
                ExecutionEngine.PutVariable(prefix + "SmoothnessStartDifference", this.SimulationSettings.SmoothnessStartDifference);
                ExecutionEngine.PutVariable(prefix + "ResponsivenessClose", this.SimulationSettings.ResponsivenessClose);

                setupDone = true;
            }
            else throw new FM4CCException("An execution instance is already under way. Please tear down the environment first!");
        }
    
        public override void TearDownEnvironment(bool relinquishExectionEngineControl = true)
        {
            // Clear the workspace
//            ExecutionEngine.ExecuteCommand("clear all;");
            if (relinquishExectionEngineControl)
            {
                ExecutionEngine.RelinquishProcess();
            }
            setupDone = false;
        }

        public override IList<string> GetSteps()
        {
            return new List<string>() { "RandomExploration", "WorstCaseSearch" };
        }

        public override TimeSpan GetEstimatedDuration(string step)
        {
            switch(step)
            {
                case "RandomExploration": 
                    return GetRandomExplorationEstimatedRunningTime();
                case "WorstCaseSearch":
                    return GetSingleStateEstimatedRunningTime();
                default:
                    return new TimeSpan(1000);
            }

        }

        public override object Run(string step, params object[] args)
        {
            switch(step)
            {
                case "TestRun":
                    return ExecuteSimulationRun();
                case "RandomExploration": 
                    return ExecuteRandomExploration();
                case "WorstCaseSearch":
                    return ExecuteWorstCaseSearch();
                default:
                    throw new FM4CCException("No such step exists");
            }
        }

        public override bool Run(FaultModelTesterTestCase testCase)
        {
            IDictionary<string, object> input = testCase.Input;
            SetTestRunParameters((double)input["Initial"], (double)input["Final"]);
            return (bool)ExecuteSimulationRun();
        }


        public override string Name { get { return name; } }
        public override string Description { get { return description; } }
        public override string ToString()
        {
            return shortName;
        }

#endregion

        #region Two Steps Fault Model
        
        public BackgroundWorker RandomExplorationWorker { get; protected set; }
        public BackgroundWorker WorstCaseWorker { get; protected set; }
        public BackgroundWorker TestRunWorker { get; protected set; }
        
        #endregion

        #region Simulation

        private double initial;
        private double final;

        internal void SetTestRunParameters(double initial, double final)
        {
            this.initial = initial;
            this.final = final;
        }

        private object ExecuteSimulationRun()
        {
            if (setupDone)
            {
                ExecutionEngine.ChangeWorkingFolder(scriptsPath + "\\ModelExecution");
                
                ExecutionEngine.PutVariable(prefix + "InitialDesiredValue", initial);
                ExecutionEngine.PutVariable(prefix + "DesiredValue", final);

                string message = ExecutionEngine.ExecuteCommand("StepModelExecution");
                
                return message.ToLower().Contains("success");
            }
            else throw new InvalidOperationException("Setup not performed");
        }

        #endregion

        #region Random Exploration

        private string ExecuteRandomExploration()
        {
            if (setupDone)
            {
                ExecutionEngine.ChangeWorkingFolder(scriptsPath + "\\RandomExploration");

                return ExecutionEngine.ExecuteCommand("RandomExploration_Step");
            }
            else throw new InvalidOperationException("Setup not performed");
        }

        private TimeSpan GetRandomExplorationEstimatedRunningTime()
        {
            // get number of cores, since MATLAB's Parallel Computing Toolbox typically starts the same amount of workers
            int coreCount = 0;
            foreach (var item in new System.Management.ManagementObjectSearcher("Select * from Win32_Processor").Get())
            {
                coreCount += int.Parse(item["NumberOfCores"].ToString());
            }

            double time = SimulationSettings.ModelRunningTime * (double)this.FaultModelConfiguration.GetValue("Regions") * (double)this.FaultModelConfiguration.GetValue("Regions") * (double)this.FaultModelConfiguration.GetValue("PointsPerRegion") / (double)coreCount;

            TimeSpan result = new TimeSpan(0, 0, (int)(time));
            return result;
        }


        #endregion

        #region SingleStateSearch

        private object ExecuteWorstCaseSearch()
        {
            if (setupDone)
            {
                ExecutionEngine.ChangeWorkingFolder(scriptsPath + "\\SingleStateSearch");

                return ExecutionEngine.ExecuteCommand("SingleStateSearch_Step");
            }
            else throw new InvalidOperationException("Setup not performed");
        }

        public void SetSearchParameters(int requirement, int regionX, int regionY, HeatPoint startPoint)
        {
            if (setupDone)
            {
                ExecutionEngine.PutVariable(prefix + "MaxObjectiveFunctionIndex", requirement);
                ExecutionEngine.PutVariable(prefix + "RegionXIndex", (double)regionX);
                ExecutionEngine.PutVariable(prefix + "RegionYIndex", (double)regionY);

                ExecutionEngine.PutVariable(prefix + "StartPoint", new double[] {startPoint.X, startPoint.Y});
            }
        }
     
        private TimeSpan GetSingleStateEstimatedRunningTime()
        {
            double num = 0.0;
            List<string> requirements = this.FaultModelConfiguration.GetValue("Requirements", "complex") as List<string>;
            // TODO
            num = 0;
            
            TimeSpan result = new TimeSpan(0, 0, (int)(num));
            return result;
        }

        #endregion

    }
}
