using FM4CC.ExecutionEngine;
using FM4CC.Simulation;
using FM4CC.WPFGUI.Project;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace FM4CC.WPFGUI.Workflow.OpenSave
{
    static class OpenSaveHandler
    {
        public enum OpenProjectStatus { Opened, Canceled, Invalid };
        public static void SaveProject(TestProject testProject)
        {
            System.Windows.Forms.SaveFileDialog saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            saveFileDialog.Title = "Save test project";
            saveFileDialog.RestoreDirectory = true;
            saveFileDialog.InitialDirectory = Directory.GetCurrentDirectory();
            saveFileDialog.Filter = "Fault Model Tester Projects (*.fmpx) | *.fmpx";
            saveFileDialog.FileName = testProject.Name;

            if (saveFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string fileName = null;
                
                if (!saveFileDialog.FileName.Contains(".fmpx"))
                {
                    fileName = saveFileDialog.FileName + ".fmpx";
                }
                else
                {
                    fileName = saveFileDialog.FileName + ".fmpx";
                }

                FileStream f = File.Create(saveFileDialog.FileName);

                var listOfFaultModelAssemblies = (from lAssembly in AppDomain.CurrentDomain.GetAssemblies()
                                                  where lAssembly.FullName.Contains("FaultModel")
                                                  select lAssembly).ToArray();
                var extraTypes = (from lAssembly in listOfFaultModelAssemblies
                                                      from lType in lAssembly.GetTypes()
                                                      where lType.IsSubclassOf(typeof(FM4CC.Environment.FaultModelConfiguration))
                                                      select lType).ToArray();
                var extraTypes2 = (from lAssembly in listOfFaultModelAssemblies
                                   from lType in lAssembly.GetTypes()
                                   where lType.IsSubclassOf(typeof(FM4CC.TestCase.FaultModelTesterTestCase))
                                   select lType).ToArray();
                XmlSerializer xsSubmit = new XmlSerializer(typeof(TestProject), extraTypes.Concat(extraTypes2).ToArray());

                XmlWriter writer = XmlWriter.Create(f);
                xsSubmit.Serialize(writer, testProject);
                
                f.Close();
            }
        }

        public static OpenProjectStatus OpenProject(ref TestProject testProject)
        {
            System.Windows.Forms.OpenFileDialog openFileDialog = new System.Windows.Forms.OpenFileDialog();
            openFileDialog.Title = "Open test project";
            openFileDialog.RestoreDirectory = true;
            openFileDialog.InitialDirectory = Directory.GetCurrentDirectory();
            openFileDialog.Filter = "Fault Model Tester Projects (*.fmpx) | *.fmpx";

            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                FileStream f = File.OpenRead(openFileDialog.FileName);
                XmlReader reader = XmlReader.Create(new StreamReader(f, Encoding.GetEncoding("UTF-8")));

                var listOfFaultModelAssemblies = (from lAssembly in AppDomain.CurrentDomain.GetAssemblies()
                                                  where lAssembly.FullName.Contains("FaultModel")
                                                  select lAssembly).ToArray();
                var extraTypes = (from lAssembly in listOfFaultModelAssemblies
                                  from lType in lAssembly.GetTypes()
                                  where lType.IsSubclassOf(typeof(FM4CC.Environment.FaultModelConfiguration))
                                  select lType).ToArray();
                var extraTypes2 = (from lAssembly in listOfFaultModelAssemblies
                                   from lType in lAssembly.GetTypes()
                                   where lType.IsSubclassOf(typeof(FM4CC.TestCase.FaultModelTesterTestCase))
                                   select lType).ToArray();
                var extraType3 = new Type[] { typeof(SimulationSettings) };
                XmlSerializer xsSubmit = new XmlSerializer(typeof(TestProject), extraTypes.Concat(extraTypes2).ToArray());
                
                if (xsSubmit.CanDeserialize(reader))
                {
                    testProject = xsSubmit.Deserialize(reader) as TestProject;
                    f.Close();

                    return OpenProjectStatus.Opened;
                }
                else
                {
                    f.Close();
                    return OpenProjectStatus.Invalid;
                }
            }
            else
            {
                return OpenProjectStatus.Canceled;
            }
        }

    }
}
