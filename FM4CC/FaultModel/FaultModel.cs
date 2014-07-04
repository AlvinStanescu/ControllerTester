using FM4CC.Environment;
using FM4CC.ExecutionEngine;
using FM4CC.Simulation;
using FM4CC.TestCase;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace FM4CC.FaultModels
{
    public abstract class FaultModel
    {
        public abstract void SetUpEnvironment();
        public abstract void TearDownEnvironment(bool relinquishExectionEngineControl = true);
        public abstract IList<string> GetSteps();
        public abstract object Run(string step, params object[] args);
        public abstract bool Run(FaultModelTesterTestCase testCase);
        public abstract TimeSpan GetEstimatedDuration(string step);
        public abstract string Name { get; }
        public abstract string Description { get; }
        public abstract override string ToString();
        
        public FaultModelConfiguration FaultModelConfiguration { get; protected set; }
        public ExecutionInstance ExecutionInstance { get; set; }
        public ExecutionEnvironment ExecutionEngine { get; protected set; }
        public SimulationSettings SimulationSettings { get; set; }
    }
}
