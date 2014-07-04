using FM4CC.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FM4CC.TestCase
{
    public abstract class FaultModelTesterTestCase
    {
        public string FaultModel { get; set; }
        public string Name { get; set; }
        public SerializableDictionary<string, object> Input { get; set; }
        public bool Passed { get; set; }

        public override string ToString()
        {
            return Name;
        }

        public abstract string GetDescription();
    }
}
