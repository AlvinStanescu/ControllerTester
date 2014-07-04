using FM4CC.ExecutionEngine;
using FM4CC.Simulation;
using FM4CC.WPFGUI.Configuration;
using FM4CC.WPFGUI.ExecutionEngine;
using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace FM4CC.WPFGUI
{
    /// <summary>
    /// Interaction logic for NewProject.xaml
    /// </summary>
    public partial class NewProjectWindow : MetroWindow
    {
        private NewProjectViewModel viewModel;

        public SimulationSettings SimulationSettings { get { return viewModel.SimulationSettings; } }
        public NewProjectWindow(ICollection<ExecutionEnvironment> executionEngines, ref ExecutionInstance executionInstance, FMTesterConfiguration configuration)
        {
            InitializeComponent();

            viewModel = new NewProjectViewModel(executionEngines, ref executionInstance, configuration, controlContainer);
            this.DataContext = viewModel;
        }

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            viewModel.ShowNext(sender, this);
        }

        private void MetroWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.DialogResult = (bool?)viewModel.IsDone;
        }


    }
}
