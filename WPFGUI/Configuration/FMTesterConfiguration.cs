using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace FM4CC.WPFGUI.Configuration
{
    public class FMTesterConfiguration
    {
        public string MatLABFolderPath { get; set; }
        public string ScriptsPath { get; set; }

        public FMTesterConfiguration()
        {
#if DEBUG
            ScriptsPath = Directory.GetCurrentDirectory() +"\\..\\..\\..\\FaultModels\\Common";
#else
            ScriptsPath = Directory.GetCurrentDirectory();
#endif
        }

        public void SaveSettings(string path)
        {
            FileStream f = File.Create(path);
            XmlSerializer xsSubmit = new XmlSerializer(typeof(FMTesterConfiguration));
            XmlWriter writer = XmlWriter.Create(f);
            xsSubmit.Serialize(writer, this);

            f.Close();
        }

        public static FMTesterConfiguration LoadSettings(string path)
        {
            FMTesterConfiguration newConfiguration;

            if (!File.Exists(path)) return null;
            FileStream f = File.OpenRead(path);
            XmlReader reader = XmlReader.Create(f);
            XmlSerializer xsSubmit = new XmlSerializer(typeof(FMTesterConfiguration));
            if (xsSubmit.CanDeserialize(reader))
            {
                newConfiguration = xsSubmit.Deserialize(reader) as FMTesterConfiguration;
                f.Close();
                return newConfiguration;
            }
            else
            {
                f.Close();
                return new FMTesterConfiguration();
            }
        }
    }
}
