using FM4CC.ExecutionEngine;
using FM4CC.Simulation;
using FM4CC.WPFGUI.Configuration;
using FM4CC.WPFGUI.Workflow.Workers;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
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
    /// Interaction logic for SimulationSettingsControl.xaml
    /// </summary>
    public partial class SimulationSettingsControl : UserControl
    {
        public SimulationSettings ModelSimulationSettings { get; set; }
        private FMTesterConfiguration configuration;
        private ExecutionInstance instance;
        private ExecutionEnvironment environment;
        private MetroWindow containingWindow;

        private ProgressDialogController progressController;

        public SimulationSettingsControl(MetroWindow containingWindow, SimulationSettings settings, FMTesterConfiguration configuration, ExecutionInstance instance, ExecutionEnvironment environment)
        {            
            InitializeComponent();
            this.containingWindow = containingWindow;
            this.ModelSimulationSettings = settings;
            this.configuration = configuration;
            this.instance = instance;
            this.environment = environment;
            
            if (settings != null)
            {
                ModelSimulationTimeNumUpDown.Value = settings.ModelSimulationTime as double?;
                DesiredValueReachedNumUpDown.Value = settings.StableStartTime as double?;
                SmoothnessStartDifferenceNumUpDown.Value = settings.SmoothnessStartDifference as double?;
                ResponsivenessPercentCloseNumUpDown.Value = (100.0 - settings.ResponsivenessPercentClose) as double?;

                DesiredValueNameTextBox.Text = settings.DesiredVariable.Name;
                DesiredValueFromNumUpDown.Value = Decimal.ToDouble(settings.DesiredVariable.FromValue) as double?;
                DesiredValueToNumUpDown.Value = Decimal.ToDouble(settings.DesiredVariable.ToValue) as double?;

                ActualValueNameTextBox.Text = settings.ActualVariable.Name;
                ActualValueFromNumUpDown.Value = Decimal.ToDouble(settings.ActualVariable.FromValue) as double?;
                ActualValueToNumUpDown.Value = Decimal.ToDouble(settings.ActualVariable.ToValue) as double?;
            }
        }

        private void ModelSimulationTimeNumUpDown_GotFocus(object sender, RoutedEventArgs e)
        {
            this.DescriptionTextBlock.Text = "The simulation time of the model needed for the desired value to, at least according to the requirements, be reached and stable.";
        }

        private void DesiredValueReachedNumUpDown_GotFocus(object sender, RoutedEventArgs e)
        {
            this.DescriptionTextBlock.Text = "The time it takes until the plant model stabilizes at the chosen desired value. Must be less than the model simulation time.";
        }

        private void SmoothnessStartDifferenceNumUpDown_GotFocus(object sender, RoutedEventArgs e)
        {
            this.DescriptionTextBlock.Text = "The absolute difference between actual and desired value after which the smoothness measurement will begin. The value after which the smoothness measurement begins cannot be more than a tenth of the actual value range.";
        }

        private void ResponsivenessPercentCloseNumUpDown_GotFocus(object sender, RoutedEventArgs e)
        {
            this.DescriptionTextBlock.Text = "The % of the desired value the actual value needs to reach before the responsiveness measurement ends, percentage at which it is considered that the desired value was approximately reached. ";
        }

        private void DesiredValueNameTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            this.DescriptionTextBlock.Text = "The name of the variable exported from the workspace representing the desired value. The desired variable can have values in the interval [DesiredValueFrom, DesiredValueTo].";
        }

        private void DesiredValueFromNumUpDown_GotFocus(object sender, RoutedEventArgs e)
        {
            this.DescriptionTextBlock.Text = "The lower bound of the desired value. Must be greater or equal to the lower bound of the actual value.";
        }

        private void DesiredValueToNumUpDown_GotFocus(object sender, RoutedEventArgs e)
        {
            this.DescriptionTextBlock.Text = "The upper bound of the desired value. Should be less or equal to the upper bound of the actual value.";
        }

        private void ActualValueNameTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            this.DescriptionTextBlock.Text = "The name of the variable imported to the workspace representing the actual value. The actual variable can have values in the interval [ActualValueFrom, ActualValueTo].";
        }

        private void ActualValueFromNumUpDown_GotFocus(object sender, RoutedEventArgs e)
        {
            this.DescriptionTextBlock.Text = "The lower bound of the actual value.";

        }

        private void ActualValueToNumUpDown_GotFocus(object sender, RoutedEventArgs e)
        {
            this.DescriptionTextBlock.Text = "The upper bound of the actual value.";

        }
        
        private void Discard_Click(object sender, RoutedEventArgs e)
        {
            containingWindow.Close();
        }

        private async void Validate_Click(object sender, RoutedEventArgs e)
        {
            double modelSimulationTime = (double)this.ModelSimulationTimeNumUpDown.Value;
            double desiredValueReachedTime = (double)this.DesiredValueReachedNumUpDown.Value;
            double desiredFrom = (double)this.DesiredValueFromNumUpDown.Value;
            double desiredTo = (double)this.DesiredValueToNumUpDown.Value;
            double actualFrom = (double)this.ActualValueFromNumUpDown.Value;
            double actualTo = (double)this.ActualValueToNumUpDown.Value;
            double smoothnessDiff = (double)this.SmoothnessStartDifferenceNumUpDown.Value;
            double responsivenessClose = (double)this.ResponsivenessPercentCloseNumUpDown.Value;

            if (desiredValueReachedTime >= modelSimulationTime)
            {
                MessageBox.Show("Invalid desired value reached time. The desired value reached time cannot be greater or equal to the model simulation time.", "Invalid setting", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (desiredFrom >= desiredTo)
            {
                MessageBox.Show("Invalid desired value range", "Invalid setting", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (actualFrom >= actualTo)
            {
                MessageBox.Show("Invalid actual value range", "Invalid setting", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (actualFrom > desiredFrom)
            {
                MessageBox.Show("Invalid actual value range", "Invalid setting", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (smoothnessDiff > (Math.Abs(actualFrom - actualTo)/10))
            {
                MessageBox.Show("The value after which the smoothness measurement begins cannot be more than a tenth of the actual value range", "Invalid setting", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (responsivenessClose > 100.0 || responsivenessClose < 75.0)
            {
                MessageBox.Show("The percent at which the responsiveness measurement begins cannot be more than a tenth of the actual value range", "Invalid setting", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            SimulationParameter desired = new SimulationParameter(this.DesiredValueNameTextBox.Text, SimulationParameterType.Desired, new decimal(desiredFrom), new decimal(desiredTo));
            SimulationParameter actual = new SimulationParameter(this.ActualValueNameTextBox.Text, SimulationParameterType.Actual, new decimal(actualFrom), new decimal(actualTo));
            ModelSimulationSettings = new SimulationSettings(modelSimulationTime, desiredValueReachedTime, smoothnessDiff, 100.0 - responsivenessClose, desired, actual);

            progressController = await containingWindow.ShowProgressAsync("Please wait...", "Model simulation running");
            progressController.SetIndeterminate();
            
            TestFunctionality();
        }

        private void TestFunctionality()
        {
            SimulationWorker simulationWorker = new SimulationWorker(instance, environment, this.ModelSimulationSettings, configuration);
            simulationWorker.RunWorkerCompleted += simulationWorker_RunWorkerCompleted;
            
            simulationWorker.RunWorkerAsync();
        }

        async void simulationWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            string message = (string)e.Result;
            
            await progressController.CloseAsync();

            if (message.ToLower().Contains("success"))
            {
                await containingWindow.ShowMessageAsync("Successful simulation", "The model simulation was completed successfully. The total model running time was " + ModelSimulationSettings.ModelRunningTime + " seconds.", MessageDialogStyle.Affirmative);
                containingWindow.Close();
            }
            else
            {
                await containingWindow.ShowMessageAsync("Invalid setting", "The model simulation failed with error:\r\n\r\n" + message, MessageDialogStyle.Affirmative);
            }
        }
    }
}
