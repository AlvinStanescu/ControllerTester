using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace FM4CC.Environment
{
    public abstract class FaultModelConfiguration : IXmlSerializable
    {
        public string FaultModelName { get; set; }

        public abstract object GetValue(string name, string collection = "primitives");
        public abstract void SetValue(string name, object value, string collection = "primitives");
        public abstract Dictionary<string, object>.Enumerator GetParametersEnumerator(string collection = "primitives");
        public abstract System.Xml.Schema.XmlSchema GetSchema();
        public abstract void ReadXml(System.Xml.XmlReader reader);
        public abstract void WriteXml(System.Xml.XmlWriter writer);
    }
}
