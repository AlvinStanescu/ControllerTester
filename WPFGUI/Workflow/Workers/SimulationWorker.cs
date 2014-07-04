using FM4CC.ExecutionEngine;
using FM4CC.FaultModels;
using FM4CC.Simulation;
using FM4CC.WPFGUI.Configuration;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace FM4CC.WPFGUI.Workflow.Workers
{
    public class SimulationWorker : BackgroundWorker
    {
        ExecutionInstance instance;
        ExecutionEnvironment environment;
        SimulationSettings settings;
        FMTesterConfiguration configuration;
        System.Diagnostics.Process process;
        string scriptsPath;

        public SimulationWorker(ExecutionInstance instance, ExecutionEnvironment environment, SimulationSettings settings, FMTesterConfiguration configuration) : base()
        {
            this.configuration = configuration;
            this.scriptsPath = configuration.ScriptsPath;
            this.instance = instance;
            this.environment = environment;
            this.settings = settings;

            this.DoWork += simulationWorker_DoWork;
        }

        private void simulationWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            double duration = -1;
            string prefix = "CT_";
            string message;

            environment.AcquireProcess();

            // Clear the workspace
            environment.ExecuteCommand("clear all;");

            environment.PutVariable(prefix + "ScriptsPath", scriptsPath);

            string modelFile = instance.GetValue("SUTPath");
            string modelPath = modelFile.Substring(0, modelFile.LastIndexOf('\\'));
            string modelName = modelFile.Substring(modelFile.LastIndexOf('\\')+1);
            modelName = modelName.Substring(0, modelName.LastIndexOf('.'));

            environment.PutVariable(prefix + "AccelerationDisabled", false);
            environment.PutVariable(prefix + "ModelFile", modelFile);
            environment.PutVariable(prefix + "ModelPath", modelPath);
            environment.PutVariable(prefix + "ModelName", modelName);
            environment.PutVariable(prefix + "ModelConfigurationFile", instance.GetValue("SUTSettingsPath"));
            environment.PutVariable(prefix + "ModelSimulationTime", this.settings.ModelSimulationTime);

            environment.PutVariable(prefix + "DesiredValueRangeStart", Convert.ToDouble(this.settings.DesiredVariable.FromValue));
            environment.PutVariable(prefix + "DesiredValueRangeEnd", Convert.ToDouble(this.settings.DesiredVariable.ToValue));
            environment.PutVariable(prefix + "DesiredVariableName", this.settings.DesiredVariable.Name);

            environment.PutVariable(prefix + "ActualValueRangeStart", Convert.ToDouble(this.settings.ActualVariable.FromValue));
            environment.PutVariable(prefix + "ActualValueRangeEnd", Convert.ToDouble(this.settings.ActualVariable.ToValue));
            environment.PutVariable(prefix + "ActualVariableName", this.settings.ActualVariable.Name);

            environment.PutVariable(prefix + "InitialDesiredValue", Convert.ToDouble(this.settings.DesiredVariable.FromValue));
            environment.PutVariable(prefix + "DesiredValue", Convert.ToDouble(this.settings.DesiredVariable.ToValue));

            environment.PutVariable(prefix + "TimeStable", this.settings.StableStartTime);
            environment.PutVariable(prefix + "SmoothnessStartDifference", this.settings.SmoothnessStartDifference);
            environment.PutVariable(prefix + "ResponsivenessPercentClose", this.settings.ResponsivenessPercentClose);

            environment.ChangeWorkingFolder(scriptsPath + "\\ModelExecution");
            environment.ExecuteCommand("save('settings')");
            process = new System.Diagnostics.Process();
            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
            startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;

            if (configuration.MatLABFolderPath != null && configuration.MatLABFolderPath != "")
            {
                startInfo.FileName = configuration.MatLABFolderPath + "\\bin\\matlab.exe";
            }
            else
            {
                startInfo.FileName = "matlab.exe";
            }

            startInfo.Arguments = "-nodisplay -nosplash -nodesktop -r ";
            startInfo.Arguments += "\"run('" + scriptsPath + "\\ModelExecution\\AccelerationBuilder.m');\"";
            process.StartInfo = startInfo;
            process.Start();

            process.CloseMainWindow();

            while (!File.Exists(scriptsPath + "\\ModelExecution\\compile.done"))
            {
                Thread.Sleep(10);
            }
            File.Delete(scriptsPath + "\\ModelExecution\\compile.done");

            message = environment.ExecuteCommand("TestModelExecution");

            string searchPattern = "runningTime=([-+]?[0-9]*\\.?[0-9]+)";
            Regex regex = new Regex(searchPattern);
            Match match = regex.Match(message);

            if (match.Groups.Count == 2)
            {
                // the system is run using a step desired value signal
                // this results in the simulation time being double the normally set simulation time
                // therefore the default duration is half
                duration = Double.Parse(match.Groups[1].Value, CultureInfo.InvariantCulture) / 2.0;
                settings.ModelRunningTime = duration;
            }

            environment.RelinquishProcess();

            e.Result = message;

        }

    }
}
