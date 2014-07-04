using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using System.Linq;

namespace FM4CC.Util
{

    [XmlRoot("dictionary")]
    public class SerializableDictionary<TKey, TValue>
        : Dictionary<TKey, TValue>, IXmlSerializable
    {
        #region IXmlSerializable Members
        public System.Xml.Schema.XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(System.Xml.XmlReader reader)
        {
            var listOfFaultModelAssemblies = (from lAssembly in AppDomain.CurrentDomain.GetAssemblies()
                                              where lAssembly.FullName.Contains("FaultModel")
                                              select lAssembly).ToArray();
            var listOfFaultModelConfigurations = (from lAssembly in listOfFaultModelAssemblies
                                                  from lType in lAssembly.GetTypes()
                                                  where lType.IsSubclassOf(typeof(FM4CC.Environment.FaultModelConfiguration))
                                                  select lType).ToArray();

            XmlSerializer keySerializer = new XmlSerializer(typeof(TKey));
            XmlSerializer valueSerializer = new XmlSerializer(typeof(TValue));

            bool wasEmpty = reader.IsEmptyElement;
            reader.Read();

            if (wasEmpty)
                return;

            while (reader.NodeType != System.Xml.XmlNodeType.EndElement)
            {
                reader.ReadStartElement("item");

                reader.ReadStartElement("key");
                TKey key = (TKey)keySerializer.Deserialize(reader);
                reader.ReadEndElement();

                string keyString = key as string;

                if (keyString != null && keyString != "FaultModelTesterProject" && keyString.Contains("FaultModel"))
                {
                    Type subType = listOfFaultModelConfigurations.First(a => a.Name.Contains(keyString));
                    valueSerializer = new XmlSerializer(subType);
                }

                reader.ReadStartElement("value");
                TValue value = (TValue)valueSerializer.Deserialize(reader);
                reader.ReadEndElement();

                this.Add(key, value);

                reader.ReadEndElement();
                reader.MoveToContent();
            }
            reader.ReadEndElement();
        }

        public void WriteXml(System.Xml.XmlWriter writer)
        {
            var listOfFaultModelAssemblies = (from lAssembly in AppDomain.CurrentDomain.GetAssemblies()
                                              where lAssembly.FullName.Contains("FaultModel")
                                              select lAssembly).ToArray();
            var listOfFaultModelConfigurations = (from lAssembly in listOfFaultModelAssemblies
                                                  from lType in lAssembly.GetTypes()
                                                  where lType.IsSubclassOf(typeof(FM4CC.Environment.FaultModelConfiguration))
                                                  select lType).ToArray();

            XmlSerializer keySerializer = new XmlSerializer(typeof(TKey));
            XmlSerializer valueSerializer = new XmlSerializer(typeof(TValue), listOfFaultModelConfigurations);

            foreach (TKey key in this.Keys)
            {
                writer.WriteStartElement("item");

                writer.WriteStartElement("key");
                keySerializer.Serialize(writer, key);
                writer.WriteEndElement();

                string keyString = key as string;

                if (keyString != null && keyString != "FaultModelTesterProject" && keyString.Contains("FaultModel"))
                {
                    Type subType = listOfFaultModelConfigurations.First(a => a.Name.Contains(keyString));
                    valueSerializer = new XmlSerializer(subType);
                }

                writer.WriteStartElement("value");
                TValue value = this[key];
                valueSerializer.Serialize(writer, value);
                writer.WriteEndElement();

                writer.WriteEndElement();
            }
        }
        #endregion
    }
}