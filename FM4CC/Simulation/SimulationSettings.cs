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
        public double ResponsivenessPercentClose { get; set; }
        public SimulationParameter DesiredVariable { get; set; }
        public SimulationParameter ActualVariable { get; set; }
                

        public SimulationSettings(double modelSimTime, double stableStartTime, double startDiff, double respPercentClose, SimulationParameter desired, SimulationParameter actual)
        {
            this.ModelSimulationTime = modelSimTime;
            this.StableStartTime = stableStartTime;
            this.SmoothnessStartDifference = startDiff;
            this.ResponsivenessPercentClose = respPercentClose;
            this.DesiredVariable = desired;
            this.ActualVariable = actual;
        }

        public SimulationSettings()
        {

        }

    }
}
