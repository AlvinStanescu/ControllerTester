using FM4CC.Environment;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace FM4CC.FaultModels.Step
{
    public class StepFaultModelConfiguration : FaultModelConfiguration
    {
        private Dictionary<string, object> complexParameters;
        private Dictionary<string, object> primitiveParameters;

        public StepFaultModelConfiguration()
        {
            this.FaultModelName = "StepFaultModel";
            complexParameters = new Dictionary<string, object>();
            primitiveParameters = new Dictionary<string, object>();

            primitiveParameters.Add("Regions", (double)10);
            primitiveParameters.Add("PointsPerRegion", (double)10);
            primitiveParameters.Add("UseAdaptiveRandomSearch", true);
            complexParameters.Add("Requirements", new List<string>() { "Stability", "Liveness", "Smoothness", "Responsiveness", "Oscillation" });
            primitiveParameters.Add("OptimizationAlgorithm", "SimulatedAnnealing");
        }

        public override object GetValue(string name, string collection = "primitives")
        {
            object value;

            if (collection == "primitives")
            {
                if (primitiveParameters.TryGetValue(name, out value))
                {
                    return value;
                }
                else
                {
                    throw new ArgumentException("Invalid parameter name for the Step Fault Model - " + name);
                }
            }
            else if (collection == "complex")
            {
                if (complexParameters.TryGetValue(name, out value))
                {
                    return value;
                }
                else
                {
                    throw new ArgumentException("Invalid parameter name for the Step Fault Model - " + name);
                }
            }
            else throw new ArgumentException("Collection " + collection + " does not exist");         
        }

        public override void SetValue(string name, object value, string collection = "primitives")
        {
            if (collection == "primitives")
            {
                primitiveParameters[name] = value;
            }
            else
            {
                if (collection == "complex")
                {
                    complexParameters[name] = value;
                }
                else throw new ArgumentException("Collection " + collection + " does not exist");
            }
        }

        public override Dictionary<string, object>.Enumerator GetParametersEnumerator(string collection = "primitives")
        {
            if (collection == "primitives") return primitiveParameters.GetEnumerator();
            else if (collection == "complex") return complexParameters.GetEnumerator();
            else throw new ArgumentException("Collection " + collection + " does not exist");
        }
                
        public override System.Xml.Schema.XmlSchema GetSchema()
        {
            return null;
        }

        public override void ReadXml(XmlReader reader)
        {
            complexParameters.Clear();
            primitiveParameters.Clear();

            reader.MoveToContent();

            reader.ReadStartElement();

            primitiveParameters.Add("Regions", reader.ReadElementContentAsDouble());
            primitiveParameters.Add("PointsPerRegion", reader.ReadElementContentAsDouble());

            reader.MoveToFirstAttribute();

            int cnt = Int32.Parse(reader.GetAttribute("Count"));
            reader.ReadToFollowing("Requirement");
            IList<string> requirements = new List<string>();

            for (int i = 0; i < cnt; i++)
            {
                requirements.Add(reader.ReadElementContentAsString());
            }

            complexParameters.Add("Requirements", requirements);

            reader.ReadEndElement();

            primitiveParameters.Add("UseAdaptiveRandomSearch", reader.ReadElementContentAsBoolean());
            primitiveParameters.Add("OptimizationAlgorithm", reader.ReadElementContentAsString());
            reader.ReadEndElement();

        }

        public override void WriteXml(XmlWriter writer)
        {
            writer.WriteStartElement("Regions");
            writer.WriteValue((double)primitiveParameters["Regions"]);
            writer.WriteEndElement();

            writer.WriteStartElement("PointsPerRegion");
            writer.WriteValue((double)primitiveParameters["PointsPerRegion"]);
            writer.WriteEndElement();

            writer.WriteStartElement("Requirements");

            IList<string> requirements = (IList<string>)complexParameters["Requirements"];
            writer.WriteStartAttribute("Count");
            writer.WriteValue(requirements.Count);
            writer.WriteEndAttribute();

            foreach (string requirement in requirements)
            {
                writer.WriteElementString("Requirement", requirement);
            }
            writer.WriteEndElement();

            writer.WriteStartElement("UseAdaptiveRandomSearch");
            writer.WriteValue((bool)primitiveParameters["UseAdaptiveRandomSearch"]);
            writer.WriteEndElement();

            writer.WriteStartElement("OptimizationAlgorithm");
            writer.WriteValue((string)primitiveParameters["OptimizationAlgorithm"]);
            writer.WriteEndElement();
        }
    }
}
