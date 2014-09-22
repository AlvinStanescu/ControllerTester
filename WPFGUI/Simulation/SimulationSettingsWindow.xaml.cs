using FM4CC.ExecutionEngine;
using FM4CC.Simulation;
using FM4CC.WPFGUI.Configuration;
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
using System.Windows.Shapes;

namespace FM4CC.WPFGUI.Simulation
{
    /// <summary>
    /// Interaction logic for SimulationSettingsWindow.xaml
    /// </summary>
    public partial class SimulationSettingsWindow : MetroWindow
    {
        public SimulationSettings ModelSimulationSettings { get { return simulationControl.ModelSimulationSettings; } }
        private SimulationSettingsControl simulationControl;

        public SimulationSettingsWindow(SimulationSettings settings, FMTesterConfiguration configuration, ExecutionInstance instance, ExecutionEnvironment environment)
        {
            InitializeComponent();
            this.EnableDWMDropShadow = true;
            this.ShowMinButton = false;
            this.ShowMaxRestoreButton = false;

            simulationControl = new SimulationSettingsControl(this, settings, configuration, instance, environment);
            this.Container.Children.Add(simulationControl);
        }
    }
}
