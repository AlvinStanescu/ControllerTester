using FM4CC.Environment;
using FM4CC.Simulation;
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
    /// Interaction logic for ConfigurationControl.xaml
    /// </summary>
    public partial class ConfigurationControl : UserControl, IConfigurationControl
    {
        StepFaultModelConfiguration configuration;
        SimulationSettings simulationSettings;

        public ConfigurationControl(FaultModelConfiguration configuration, SimulationSettings simulationSettings)
        {
            InitializeComponent();
            LoadConfiguration(configuration);
            this.simulationSettings = simulationSettings;

        }

        public bool Validate()
        {
            if (this.NumberHeatMapRegionsNumUpDown.Value != null && ((double)this.NumberHeatMapRegionsNumUpDown.Value) <= 0)
            {
                MessageBox.Show("Invalid setting", "Invalid heat map divison factor", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            if (this.PointsPerRegionNumUpDown.Value != null && ((double)this.PointsPerRegionNumUpDown.Value) <= 0)
            {
                MessageBox.Show("Invalid setting", "Invalid no. of points per region", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            Save();

            return true;
        }

        public void Save()
        {
            configuration.SetValue("Regions", Convert.ToDouble(Math.Abs((double)this.NumberHeatMapRegionsNumUpDown.Value)));
            configuration.SetValue("PointsPerRegion", Convert.ToDouble(Math.Abs((double)this.PointsPerRegionNumUpDown.Value)));

            configuration.SetValue("UseAdaptiveRandomSearch", ((string)this.ExplorationAlgorithmComboBox.SelectedItem) == "AdaptiveRandomSearch" ? true : false);
            configuration.SetValue("OptimizationAlgorithm", this.LocalSeachAlgorithmComboBox.SelectedItem);

            List<string> requirements = new List<string>();

            foreach (CheckBox cb in RequirementsStackPanel.Children)
            {
                if ((bool)cb.IsChecked) requirements.Add(cb.Content as string);
            }
            configuration.SetValue("Requirements", requirements, "complex");
        }

        private void LoadConfiguration(FaultModelConfiguration configuration)
        {
            this.configuration = (StepFaultModelConfiguration)configuration;

            this.ExplorationAlgorithmComboBox.Items.Add("AdaptiveRandomSearch");
            this.ExplorationAlgorithmComboBox.Items.Add("RandomSearch");

            this.LocalSeachAlgorithmComboBox.Items.Add("SimulatedAnnealing");
            this.LocalSeachAlgorithmComboBox.Items.Add("PatternSearch");
            this.LocalSeachAlgorithmComboBox.Items.Add("MultiStart");
            this.LocalSeachAlgorithmComboBox.Items.Add("GlobalSearch");
            this.LocalSeachAlgorithmComboBox.Items.Add("GeneticAlgorithm");

            ReloadConfiguration();
        }

        public void ReloadConfiguration()
        {
            List<string> requirements = configuration.GetValue("Requirements", "complex") as List<string>;

            this.ExplorationAlgorithmComboBox.SelectedIndex = ((bool)configuration.GetValue("UseAdaptiveRandomSearch"))?0:1;
            this.LocalSeachAlgorithmComboBox.SelectedValue = (string)configuration.GetValue("OptimizationAlgorithm");

            foreach (string requirement in requirements)
            {
                switch (requirement)
                {
                    case "Stability":
                        StabilityCheckBox.IsChecked = true;
                        break;
                    case "Precision":
                        PrecisionCheckBox.IsChecked = true;
                        break;
                    case "Smoothness":
                        SmoothnessCheckBox.IsChecked = true;
                        break;
                    case "Responsiveness":
                        ResponsivenessCheckBox.IsChecked = true;
                        break;
                    case "Steadiness":
                        SteadinessCheckBox.IsChecked = true;
                        break;
                    case "NormalizedSmoothness":
                        NormalizedSmoothnessCheckBox.IsChecked = true;
                        break;
                    default:
                        break;
                }
            }

            this.NumberHeatMapRegionsNumUpDown.Value = (double)configuration.GetValue("Regions") as double?;
            this.PointsPerRegionNumUpDown.Value = (double)configuration.GetValue("PointsPerRegion") as double?;
        }
    }
}
