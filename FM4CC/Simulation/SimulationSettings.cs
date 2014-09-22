using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace FM4CC.Simulation
{
    public class SimulationSettings
    {
        public double ModelRunningTime { get; set; }
        public double ModelSimulationTime { get; set; }
        public double StableStartTime { get; set; }
        public double SmoothnessStartDifference { get; set; }
        public double ResponsivenessClose { get; set; }
        public SimulationParameter DesiredVariable { get; set; }
        public SimulationParameter ActualVariable { get; set; }
        public SimulationParameter DisturbanceVariable { get; set; }
        public ModelRegressionSettings RegressionSettings { get; set; }

        public SimulationSettings(double modelSimTime, double stableStartTime, double startDiff, double respClose, SimulationParameter desired, SimulationParameter actual, SimulationParameter disturbance, ModelRegressionSettings regressionSettings)
        {
            this.ModelSimulationTime = modelSimTime;
            this.StableStartTime = stableStartTime;
            this.SmoothnessStartDifference = startDiff;
            this.ResponsivenessClose = respClose;
            this.DesiredVariable = desired;
            this.ActualVariable = actual;
            this.DisturbanceVariable = disturbance;
            this.RegressionSettings = regressionSettings;
        }

        public SimulationSettings()
        {
            DisturbanceVariable = new SimulationParameter("",SimulationParameterType.Disturbance, 0, 0);
            RegressionSettings = new ModelRegressionSettings();
        }

    }
}
