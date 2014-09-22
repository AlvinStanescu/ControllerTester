using FM4CC.Environment;
using FM4CC.ExecutionEngine;
using FM4CC.Simulation;
using FM4CC.TestCase;
using FM4CC.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace FM4CC.WPFGUI.Project
{
    [XmlRoot(ElementName = "FaultModelTesterProject")]
    public class TestProject
    {
        public TestProject()
        {
        }

        public TestProject(ExecutionInstance instance, SimulationSettings simSettings)
        {
            this.Instance = instance;
            this.ModelSimulationSettings = simSettings;
            this.TestCases = new List<FaultModelTesterTestCase>();
        }

        internal string Path
        {
            get;
            set;
        }
        internal string Name
        {
            get
            {
                return Instance.Name;
            }
            set
            {
                Instance.Name = value;
            }
        }

        public ExecutionInstance Instance { get; set; }

        public SerializableDictionary<string, FaultModelConfiguration> FaultModelSettings { get; set; }

        public SimulationSettings ModelSimulationSettings { get; set; }
        public List<FaultModelTesterTestCase> TestCases { get; set; }

    }
}
