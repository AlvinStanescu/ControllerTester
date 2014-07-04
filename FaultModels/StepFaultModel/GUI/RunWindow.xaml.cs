using FM4CC.Util;
using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FM4CC.FaultModels.Step.GUI
{
    /// <summary>
    /// Interaction logic for RunWindow.xaml
    /// </summary>
    public partial class RunWindow : MetroWindow
    {
        private StepFaultModel faultModel;
        private ProgressWindow progressWindow;
        private Action<string> log;

        public RunWindow(StepFaultModel faultModel, Action<string> logFunction)
        {
            InitializeComponent();
            
            this.log = logFunction;
            this.faultModel = faultModel;

            this.EnableDWMDropShadow = true;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            TestRunWorker testRunWorker = (TestRunWorker)faultModel.TestRunWorker;
            testRunWorker.InitialDesired = (double)InitialValueNumUpDown.Value;
            testRunWorker.FinalDesired = (double)FinalValueNumUpDown.Value;
            testRunWorker.RunWorkerCompleted += TestRunWorker_RunWorkerCompleted;

            progressWindow = new ProgressWindow("Running the model, please wait...", " Estimated progress: ");
            progressWindow.ShowCloseButton = false;
            testRunWorker.ProgressChanged += progressWindow.ProgressChanged;
            testRunWorker.RunWorkerAsync(faultModel);
            progressWindow.ShowDialog(this);

            log("Step Fault Model - Ran model with initial desired value " + testRunWorker.InitialDesired + " and final desired value " + testRunWorker.FinalDesired);

        }

        private void TestRunWorker_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            progressWindow.Close();
        }
    }
}
