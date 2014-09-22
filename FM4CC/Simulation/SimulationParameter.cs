using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace FM4CC.Simulation
{
    public enum SimulationParameterType { Actual, Desired, Disturbance, Configuration };

    public class SimulationParameter
    {
        public string Name { get; set; }
        public SimulationParameterType Type { get; set; }
        public decimal FromValue { get; set; }
        public decimal ToValue { get; set; }

        public SimulationParameter()
        {
            this.Name = "";
            this.Type = SimulationParameterType.Configuration;
            this.FromValue = 0;
            this.ToValue = 0;
        }

        public SimulationParameter(SimulationParameterType type)
        {
            this.Name = "";
            this.Type = type;
            this.FromValue = 0;
            this.ToValue = 0;
        }

        public SimulationParameter(string name, SimulationParameterType type, decimal fromValue, decimal toValue)
        {
            this.Name = name;
            this.Type = type;
            this.FromValue = fromValue;
            this.ToValue = toValue;
        }
    }
}
