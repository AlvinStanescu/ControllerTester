using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FM4CC.WPFGUI.Configuration
{
    public class MatlabInstallation
    {
        public string MatlabPath { get; set; }
        public string Name { get; set; }

        public MatlabInstallation(string path)
        {
            Name = "MATLAB " + path.Substring(path.LastIndexOf('\\') + 1);
            Name += Directory.Exists(path + "\\bin\\win64") ? " (win64)" : " (win32)";
            MatlabPath = path;
        }

        public static List<MatlabInstallation> GetMatlabInstallations()
        {
            var matlabs = GetInstallations();
            var matlabInstalls = new List<MatlabInstallation>();

            foreach (string path in matlabs)
            {
                matlabInstalls.Add(new MatlabInstallation(path));
            }

            return matlabInstalls;
        }

        private static List<string> GetInstallations()
        {
            if (!Directory.Exists("C:\\Program Files\\MATLAB\\")) return new List<string>();
            else
            {
                // at least one version of MATLAB is available
                var matlabsNormal = Directory.GetDirectories("C:\\Program Files\\MATLAB\\");
                if (Directory.Exists("C:\\Program Files (x86)\\MATLAB\\"))
                {
                    var matlabsX86 = Directory.GetDirectories("C:\\Program Files (x86)\\MATLAB\\");
                    return matlabsNormal.Concat(matlabsX86).ToList();
                }
                else return matlabsNormal.ToList();

            }

        }
    }

}
