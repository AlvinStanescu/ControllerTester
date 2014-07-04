using FM4CC.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace FM4CC.ExecutionEngine
{
    public class ExecutionInstance
    {
        public SerializableDictionary<string, string> Container { get; set; }

        public ExecutionInstance()
        {
            Container = new SerializableDictionary<string, string>();
        }

        public ExecutionInstance(string name, string environment)
        {
            this.Name = name;
            this.Environment = environment;
            Container = new SerializableDictionary<string, string>();

        }

        public bool HasKey(string key)
        {
            return Container.ContainsKey(key);
        }

        public string GetValue(string key)
        {
            return Container[key];
        }

        public void PutValue(string key, string value)
        {
            if (Container.ContainsKey(key))
            {
                Container.Remove(key);
            }

            Container.Add(key, value);
        }

        public string Name { get; set; }
        public string Environment { get; set; }
    }
}
