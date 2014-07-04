using FM4CC.TestCase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FM4CC.FaultModels.Step
{
    public class StepFaultModelTestCase : FaultModelTesterTestCase
    {
        public override string GetDescription()
        {
            return "Initial Desired Value - " + Input["Initial"] + ", Desired Value - " + Input["Final"];
        }
    }
}
