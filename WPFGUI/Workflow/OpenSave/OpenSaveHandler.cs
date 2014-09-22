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
        public static void SaveProject(TestProject testProject, bool showDialogAlways = false)
        {
            System.Windows.Forms.SaveFileDialog saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            saveFileDialog.Title = "Save test project";
            saveFileDialog.RestoreDirectory = true;
            saveFileDialog.InitialDirectory = Directory.GetCurrentDirectory();
            saveFileDialog.Filter = "Controller Tester Projects (*.fmpx) | *.fmpx";
            saveFileDialog.FileName = testProject.Name;

            if (showDialogAlways == true || testProject.Path == null)
            {
                if (saveFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    if (!saveFileDialog.FileName.Contains(".fmpx"))
                    {
                        testProject.Name = Path.GetFileNameWithoutExtension(saveFileDialog.FileName);
                        testProject.Path = saveFileDialog.FileName + ".fmpx";
                    }
                    else
                    {
                        testProject.Name = Path.GetFileNameWithoutExtension(saveFileDialog.FileName);
                        testProject.Path = saveFileDialog.FileName;
                    }
                }
                else
                {
                    return;
                }
            }

            if (testProject.Path != null)
            {
                FileStream f = File.Create(testProject.Path);

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

        private static OpenProjectStatus OpenExistingProject(ref TestProject testProject, string path)
        {
            FileStream f = File.OpenRead(path);
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
                testProject.Path = path;
                f.Close();

                return OpenProjectStatus.Opened;
            }
            else
            {
                f.Close();
                return OpenProjectStatus.Invalid;
            }
        }

        public static OpenProjectStatus OpenProject(ref TestProject testProject, string path = null)
        {
            if (path == null)
            {
                System.Windows.Forms.OpenFileDialog openFileDialog = new System.Windows.Forms.OpenFileDialog();
                openFileDialog.Title = "Open test project";
                openFileDialog.RestoreDirectory = true;
                openFileDialog.InitialDirectory = Directory.GetCurrentDirectory();
                openFileDialog.Filter = "Controller Tester Projects (*.fmpx) | *.fmpx";

                if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    return OpenExistingProject(ref testProject, openFileDialog.FileName);
                }
                else
                {
                    return OpenProjectStatus.Canceled;
                }
            }
            else
            {
                return OpenExistingProject(ref testProject, path);
            }
        }

    }
}
