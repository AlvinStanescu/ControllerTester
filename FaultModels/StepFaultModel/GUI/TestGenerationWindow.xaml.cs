using FM4CC.Environment;
using FM4CC.ExecutionEngine;
using FM4CC.FaultModels;
using FM4CC.FaultModels.Step.Parsers;
using FM4CC.Simulation;
using FM4CC.TestCase;
using FM4CC.Util;
using FM4CC.Util.Heatmap;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
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

namespace FM4CC.FaultModels.Step.GUI
{
    /// <summary>
    /// Interaction logic for TestGenerationWindow.xaml
    /// </summary>
    public partial class TestGenerationWindow : MetroWindow
    {
        private StepFaultModel faultModel;
        private static ProgressDialogController progressController;
        
        private IList<HeatMapDataSource> sources;
        private IList<HeatMapDataSource> simpleSources;
        
        private double baseUnit;
        private IList<List<DataGridHeatPoint> > explorationResults;
        private IList<TestCase.FaultModelTesterTestCase> testCases;

        private IList<string> requirements;
        private Action<string> log;

        public TestGenerationWindow(FaultModel faultModel, IList<TestCase.FaultModelTesterTestCase> testCases, Action<string> logFunction)
        {
            InitializeComponent();

            this.log = logFunction;
            this.testCases = testCases;
            this.faultModel = faultModel as StepFaultModel;
            this.EnableDWMDropShadow = true;

            this.Title = "Test Case Generation - Step Fault Model";

            requirements = (IList<string>)faultModel.FaultModelConfiguration.GetValue("Requirements", "complex");

            foreach (string requirement in requirements)
            {
                this.RequirementComboBox.Items.Add(requirement);
            }

            InvestigateWorstCaseRadioButton.IsChecked = true;

        }

        private async void TestRunWorker_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            if (progressController.IsOpen)
            {
                await progressController.CloseAsync();
            }

            if (((bool)e.Result) == false)
            {
                Exception exception = (this.faultModel.TestRunWorker as TestRunWorker).Exception;
                if (exception != null)
                {
                    log("Step Fault Model - Test case failed to run.");
                    await this.ShowMessageAsync("Test case failed to run", "The test case run failed with error:\r\n\r\n" + exception.Message, MessageDialogStyle.Affirmative);
                }
                else
                {
                    log("Step Fault Model - Test case failed to run due to a problem with the execution environment.");
                    await this.ShowMessageAsync("Test case failed to run", "Test case failed to run due to a problem with the execution environment.", MessageDialogStyle.Affirmative);
                }
            }
        }

        private async void GenerationWorker_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            if (progressController.IsOpen)
            {
                await progressController.CloseAsync();
            }

            if (((bool)e.Result) == true)
            {
                ProcessResults();
                log("Step Fault Model - Random exploration ended successfully");
            }
            else
            {
                Exception exception = (this.faultModel.RandomExplorationWorker as RandomExplorationWorker).Exception;

                if (exception != null)
                {
                    log("Step Fault Model - Random exploration failed to run");
                    await this.ShowMessageAsync("Random exploration failed", "The random exploration failed with error:\r\n\r\n" + exception.Message, MessageDialogStyle.Affirmative);
                }
                else
                {
                    log("Step Fault Model - Random exploration stopped");
                } 
                this.Close();
            }
        }

        private async void WorstCaseSearchWorker_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            if (progressController.IsOpen)
            {
                await progressController.CloseAsync();
            }

            if (((bool)e.Result) == true)
            {
                log("Step Fault Model - Worst-case Search ended successfully");
                this.DialogResult = true;
                this.Close();
                MessageBox.Show("Worst-case search completed successfully!");
            }
            else
            {
                Exception exception = (this.faultModel.WorstCaseWorker as WorstCaseScenarioWorker).Exception;
                
                if (exception != null)
                {
                    log("Step Fault Model - Worst-case Search failed to run");
                    await this.ShowMessageAsync("Worst-case Search failed", "The worst-case search failed with error:\r\n\r\n" + exception.Message, MessageDialogStyle.Affirmative);
                }
                else
                {
                    log("Step Fault Model - Worst-case Search stopped");
                }
                this.Close();
            }
        }

        public void ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            double progress = ((double)e.ProgressPercentage) / 100;
            if (progress > 1.00)
            {
                progress = 1.00;
            }

            progressController.SetProgress(progress);
            CheckIfCanceled();
        }


        private void ProcessResults()
        {
            this.WaitLabel.Visibility = System.Windows.Visibility.Hidden;
            string tempPath = Path.GetDirectoryName(faultModel.ExecutionInstance.GetValue("SUTPath")) + "\\ControllerTesterResults\\RandomExploration\\RandomExplorationPoints_Step.csv";
            string simpleTempPath = Path.GetDirectoryName(faultModel.ExecutionInstance.GetValue("SUTPath")) + "\\ControllerTesterResults\\RandomExploration\\RandomExplorationRegions_Step.csv";

            FaultModelConfiguration fmConfig = faultModel.FaultModelConfiguration;

            SimulationParameter val = faultModel.SimulationSettings.DesiredVariable;
            double divisionFactor = (double)(fmConfig.GetValue("Regions"));

            baseUnit = ((double)(val.ToValue - val.FromValue))/divisionFactor;

            sources = HeatMapParser.Parse(tempPath, val.FromValue, val.ToValue, (int)divisionFactor, val.FromValue, val.ToValue, (int)divisionFactor, requirements);
            simpleSources = SimpleHeatMapParser.Parse(simpleTempPath, val.FromValue, val.ToValue, (int)divisionFactor, val.FromValue, val.ToValue, (int)divisionFactor, requirements);
            explorationResults = new List<List<DataGridHeatPoint>>();

            for (int i = 0; i < requirements.Count; i++)
            {
                var _explorationResults = new List<DataGridHeatPoint>();

                foreach (HeatPoint hp in simpleSources[i].HeatPoints)
                {
                    _explorationResults.Add(new DataGridHeatPoint(hp as StepHeatPoint, baseUnit, Convert.ToDouble(val.FromValue), Convert.ToDouble(val.ToValue), simpleSources[i].Name));
                }

                _explorationResults.Sort(CompareIntensities);
                explorationResults.Add(_explorationResults);
            }

            RequirementComboBox.SelectedIndex = 0;
            
        }

        private void UpdateEstimation(TimeSpan duration, int numberOfTestCases)
        {
            DescriptionTextBlock.Text = "A total of " + numberOfTestCases + " test case(s) will be generated during the single state search.";

            TimeSpan fullDuration = new TimeSpan(0);
            for (int i = 0; i < numberOfTestCases; i++)
            {
                fullDuration = fullDuration.Add(duration);
            }

            StringBuilder durationBuilder = new StringBuilder(1000);
            durationBuilder.Append("It will take around ");

            if (fullDuration.Days > 0 )
            {
                durationBuilder.Append(fullDuration.Days);
                durationBuilder.Append(" day(s), ");
            }
            if (fullDuration.Hours > 0 )
            {
                durationBuilder.Append(fullDuration.Hours);
                durationBuilder.Append(" hour(s), ");
            }

            if (fullDuration.Minutes > 0 )
            {
                durationBuilder.Append(fullDuration.Minutes);
                durationBuilder.Append(" minute(s), ");
            }

            if (fullDuration.Seconds > 0 )
            {
                durationBuilder.Append(fullDuration.Seconds);
                durationBuilder.Append(" second(s), ");
            }
            durationBuilder.Remove(durationBuilder.Length-2, 2);
            durationBuilder.Append(" to finish generating all the test cases. Estimated finish time: ");

            DateTime finishTime = DateTime.Now + fullDuration;
            durationBuilder.Append(finishTime.ToShortDateString() + " " + finishTime.ToShortTimeString());

            DurationTextBlock.Text = durationBuilder.ToString();
        }

        private static int CompareIntensities(DataGridHeatPoint a, DataGridHeatPoint b)
        {
            return b.Intensity.CompareTo(a.Intensity);
        }

        private void RequirementComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            HeatMapControl.HeatMapSource = sources[RequirementComboBox.SelectedIndex];
            RegionHeatMapControl.HeatMapSource = simpleSources[RequirementComboBox.SelectedIndex];

            if (explorationResults != null)
            {
                this.DataContext = new TestGenerationViewModel(explorationResults[RequirementComboBox.SelectedIndex]);
            }
        }

        private async void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            string tempPath = Path.GetDirectoryName(faultModel.ExecutionInstance.GetValue("SUTPath")) + "\\ControllerTesterResults\\RandomExploration\\RandomExplorationPoints_Step.csv";
            string simpleTempPath = Path.GetDirectoryName(faultModel.ExecutionInstance.GetValue("SUTPath")) + "\\ControllerTesterResults\\RandomExploration\\RandomExplorationRegions_Step.csv";

            // TODO add check for same region count and points per region
            if (File.Exists(tempPath) && File.Exists(simpleTempPath) && MessageBoxResult.Yes == MessageBox.Show("Found results from a previous random exploration. Do you want to load them instead?", "Previous exploration found", MessageBoxButton.YesNo, MessageBoxImage.Question))
            {
                ProcessResults();
            }
            else
            {
                BackgroundWorker generationWorker = faultModel.RandomExplorationWorker;
                generationWorker.RunWorkerCompleted += GenerationWorker_RunWorkerCompleted;

                log("Step Fault Model - Random exploration started");

                progressController = await this.ShowProgressAsync("Performing random exploration", "Estimated progress:", true);
                generationWorker.ProgressChanged += ProgressChanged;
                generationWorker.RunWorkerAsync(faultModel);
            }
        }

        private void CheckIfCanceled()
        {
            if (progressController.IsCanceled && faultModel.RandomExplorationWorker.IsBusy && !faultModel.RandomExplorationWorker.CancellationPending)
            {
                faultModel.RandomExplorationWorker.CancelAsync();
            }

            if (progressController.IsCanceled && faultModel.WorstCaseWorker.IsBusy && !faultModel.WorstCaseWorker.CancellationPending)
            {
                faultModel.WorstCaseWorker.CancelAsync();
            }
        }

        
        private async void RunButton_Click(object sender, RoutedEventArgs e)
        {
            (sender as Button).GetValue(DataGridRow.AlternationIndexProperty);
            DataGridHeatPoint runHeatPoint = (DataGridHeatPoint)ExplorationDataGrid.SelectedValue;

            TestRunWorker testRunWorker = (TestRunWorker)faultModel.TestRunWorker;
            testRunWorker.InitialDesired = runHeatPoint.InitialDesired;
            testRunWorker.FinalDesired = runHeatPoint.FinalDesired;
            testRunWorker.RunWorkerCompleted += TestRunWorker_RunWorkerCompleted;

            progressController = await this.ShowProgressAsync("Running the model", "Please wait...");
            progressController.SetCancelable(false);
            progressController.SetIndeterminate();

            testRunWorker.RunWorkerAsync(faultModel);

            log("Step Fault Model - Ran model with initial desired value " + runHeatPoint.InitialDesired + " and final desired value " + runHeatPoint.FinalDesired);

        }

        private void ExplorationDataGrid_AutoGeneratedColumns(object sender, EventArgs e)
        {
            var dv = (ExplorationDataGrid.ItemsSource as DataView);
            dv.Sort = "Intensity";
        }

        private void SelectAll_Click(object sender, RoutedEventArgs e)
        {
            IList<DataGridHeatPoint> exploration = explorationResults[RequirementComboBox.SelectedIndex];

            foreach (DataGridHeatPoint hp in exploration)
            {
                hp.Analyze = true;
            }

            int cnt = 0;

            foreach (IList<DataGridHeatPoint> requirementList in explorationResults)
            {
                foreach (DataGridHeatPoint hp in requirementList)
                {
                    if (hp.Analyze) cnt++;
                }
            }

            UpdateEstimation(faultModel.GetEstimatedDuration("WorstCaseSearch"), (int)requirements.Count);
        }

        private void DeselectAll_Click(object sender, RoutedEventArgs e)
        {
            IList<DataGridHeatPoint> exploration = explorationResults[RequirementComboBox.SelectedIndex];

            foreach (DataGridHeatPoint hp in exploration)
            {
                hp.Analyze = false;
            }
            GenerateButton.IsEnabled = false;

            UpdateEstimation(faultModel.GetEstimatedDuration("WorstCaseSearch"), 0);
        }
        
        private void InvestigateWorstCase_Checked(object sender, RoutedEventArgs e)
        {
            AnalyzeDataGridColumn.Visibility = Visibility.Hidden;
            SelectAllButton.IsEnabled = false;
            DeselectAllButton.IsEnabled = false;

            UpdateEstimation(faultModel.GetEstimatedDuration("WorstCaseSearch"), (int) requirements.Count);

        }

        private void InvestigateSelected_Checked(object sender, RoutedEventArgs e)
        {
            AnalyzeDataGridColumn.Visibility = Visibility.Visible;
            SelectAllButton.IsEnabled = true;
            DeselectAllButton.IsEnabled = true;

            int cnt = 0;

            foreach (IList<DataGridHeatPoint> requirementList in explorationResults)
            {
                foreach (DataGridHeatPoint hp in requirementList)
                {
                    if (hp.Analyze) cnt++;
                }
            }

            UpdateEstimation(faultModel.GetEstimatedDuration("WorstCaseSearch"), cnt * requirements.Count);
        }

        private async void Generate_Click(object sender, RoutedEventArgs e)
        {
            if ((bool)InvestigateWorstCaseRadioButton.IsChecked)
            {
                log("Step Fault Model - Worst-case search started");
                WorstCaseScenarioWorker singleStateWorker = (WorstCaseScenarioWorker)faultModel.WorstCaseWorker;
                singleStateWorker.TestCases = testCases;
                singleStateWorker.RunWorkerCompleted += WorstCaseSearchWorker_RunWorkerCompleted;
                singleStateWorker.Logger = log;

                IList<DataGridHeatPoint> worstHeatPoints = new List<DataGridHeatPoint>();

                for (int req = 0; req < requirements.Count; req++)
                {
                    worstHeatPoints.Add((explorationResults[req])[0]);
                }

                singleStateWorker.SelectedRegions = worstHeatPoints;

                progressController = await this.ShowProgressAsync("Performing worst-case search", "Estimated progress:", true);

                singleStateWorker.ProgressChanged += this.ProgressChanged;
                singleStateWorker.RunWorkerAsync(faultModel);

            }
            else
            {
                IList<DataGridHeatPoint> selectedRegions = new List<DataGridHeatPoint>();

                foreach (IList<DataGridHeatPoint> requirementList in explorationResults)
                {
                    foreach (DataGridHeatPoint hp in requirementList)
                    {
                        if (hp.Analyze) selectedRegions.Add(hp);
                    }
                }

                log("Step Fault Model - Worst-case search started");

                WorstCaseScenarioWorker singleStateWorker = (WorstCaseScenarioWorker)faultModel.WorstCaseWorker;
                singleStateWorker.RunWorkerCompleted += WorstCaseSearchWorker_RunWorkerCompleted;
                singleStateWorker.SelectedRegions = selectedRegions;
                singleStateWorker.TestCases = testCases;
                singleStateWorker.Logger = log;

                progressController = await this.ShowProgressAsync("Performing worst-case search", "Estimated progress:", true);

                singleStateWorker.ProgressChanged += this.ProgressChanged;
                singleStateWorker.RunWorkerAsync(faultModel);

            }
        }
        
        private void ExplorationDataGrid_CurrentCellChanged(object sender, EventArgs e)
        {
            // TBD
            int cnt = 0;

            foreach (IList<DataGridHeatPoint> requirementList in explorationResults)
            {
                foreach (DataGridHeatPoint hp in requirementList)
                {
                    if (hp.Analyze) cnt++;
                }
            }

            if (cnt != 0) GenerateButton.IsEnabled = true;

            UpdateEstimation(faultModel.GetEstimatedDuration("WorstCaseSearch"), cnt * requirements.Count);
        }
        
    }
}
