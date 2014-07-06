using FM4CC.Environment;
using FM4CC.ExecutionEngine;
using FM4CC.FaultModels;
using FM4CC.FaultModels.Step.Parsers;
using FM4CC.Simulation;
using FM4CC.TestCase;
using FM4CC.Util;
using FM4CC.Util.Heatmap;
using MahApps.Metro.Controls;
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
        private ProgressWindow progressWindow;
        
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
            log("Step Fault Model - Random exploration started");

            requirements = (IList<string>)faultModel.FaultModelConfiguration.GetValue("Requirements", "complex");

            foreach (string requirement in requirements)
            {
                this.RequirementComboBox.Items.Add(requirement);
            }

            InvestigateWorstCaseRadioButton.IsChecked = true;

        }

        void TestRunWorker_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            progressWindow.Close();

            if (((bool)e.Result) == false)
            {
                MessageBox.Show("Failed to run the model, an unexpected error occurred.", "Model run", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        void GenerationWorker_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            progressWindow.Close();

            if (((bool)e.Result) == true)
            {
                ProcessResults();
                log("Step Fault Model - Random exploration ended successfully");
            }
            else
            {
                log("Step Fault Model - Random exploration failed with error " + e.Error.Message);
                MessageBox.Show("Generation failed, please check the settings. Error:\r\n" + e.Error.Message, "Generation failed", MessageBoxButton.OK, MessageBoxImage.Information);
                this.Close();
            }
        }

        void WorstCaseSearchWorker_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            progressWindow.Close();

            if (((bool)e.Result) == true)
            {
                log("Step Fault Model - Worst-case Search ended successfully");
                this.DialogResult = true;
                this.Close();
                MessageBox.Show("Worst-case search completed successfully!");
            }
            else
            {
                log("Step Fault Model -  Worst-case Search failed because of an unexpected error");
                this.DialogResult = false;
                this.Close();
                MessageBox.Show("Failed to run the model, an unexpected error occurred.", "Model run", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ProcessResults()
        {
            this.WaitLabel.Visibility = System.Windows.Visibility.Hidden;
            string tempPath = Path.GetDirectoryName(faultModel.ExecutionInstance.GetValue("SUTPath")) + "\\Temp\\RandomExploration\\RandomExplorationPoints_Step.csv";
            string simpleTempPath = Path.GetDirectoryName(faultModel.ExecutionInstance.GetValue("SUTPath")) + "\\Temp\\RandomExploration\\RandomExplorationRegions_Step.csv";

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
                    _explorationResults.Add(new DataGridHeatPoint(hp, baseUnit, simpleSources[i].Name));
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

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            string tempPath = Path.GetDirectoryName(faultModel.ExecutionInstance.GetValue("SUTPath")) + "\\Temp\\RandomExploration\\RandomExplorationPoints_Step.csv";
            string simpleTempPath = Path.GetDirectoryName(faultModel.ExecutionInstance.GetValue("SUTPath")) + "\\Temp\\RandomExploration\\RandomExplorationRegions_Step.csv";

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
                progressWindow = new ProgressWindow("Performing generation", " Estimated progress: ");
                progressWindow.Closed += progressWindow_Closed;
                generationWorker.ProgressChanged += progressWindow.ProgressChanged;
                generationWorker.RunWorkerAsync(faultModel);
                progressWindow.ShowDialog(this);
                progressWindow.ShowCloseButton = false;
            }
        }

        private void progressWindow_Closed(object sender, EventArgs e)
        {
            if (faultModel.RandomExplorationWorker.IsBusy)
            {
                faultModel.RandomExplorationWorker.CancelAsync();
                this.Close();
            }
        }
        
        private void RunButton_Click(object sender, RoutedEventArgs e)
        {
            (sender as Button).GetValue(DataGridRow.AlternationIndexProperty);
            DataGridHeatPoint runHeatPoint = (DataGridHeatPoint)ExplorationDataGrid.SelectedValue;

            TestRunWorker testRunWorker = (TestRunWorker)faultModel.TestRunWorker;
            testRunWorker.InitialDesired = runHeatPoint.InitialDesired;
            testRunWorker.FinalDesired = runHeatPoint.FinalDesired;
            testRunWorker.RunWorkerCompleted += TestRunWorker_RunWorkerCompleted;

            progressWindow = new ProgressWindow("Running the model, please wait...", " Estimated progress: ");
            progressWindow.ShowCloseButton = false;
            testRunWorker.ProgressChanged += progressWindow.ProgressChanged;
            testRunWorker.RunWorkerAsync(faultModel);
            progressWindow.ShowDialog(this);

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

        private void Generate_Click(object sender, RoutedEventArgs e)
        {
            if ((bool)InvestigateWorstCaseRadioButton.IsChecked)
            {
                log("Step Fault Model - Worst-case search started");
                WorstCaseScenarioWorker singleStateWorker = (WorstCaseScenarioWorker)faultModel.WorstCaseWorker;
                singleStateWorker.TestCases = testCases;
                singleStateWorker.RunWorkerCompleted += WorstCaseSearchWorker_RunWorkerCompleted;

                IList<DataGridHeatPoint> worstHeatPoints = new List<DataGridHeatPoint>();

                for (int req = 0; req < requirements.Count; req++)
                {
                    worstHeatPoints.Add((explorationResults[req])[0]);
                }

                singleStateWorker.SelectedRegions = worstHeatPoints;
                progressWindow = new ProgressWindow("Performing worst-case search", " Estimated progress: ");
                progressWindow.Closed += progressWindow_Closed;
                singleStateWorker.ProgressChanged += progressWindow.ProgressChanged;
                singleStateWorker.RunWorkerAsync(faultModel);
                progressWindow.ShowDialog(this);
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

                progressWindow = new ProgressWindow("Performing worst-case search", " Estimated progress: ");
                progressWindow.Closed += progressWindow_Closed;
                singleStateWorker.ProgressChanged += progressWindow.ProgressChanged;
                singleStateWorker.RunWorkerAsync(faultModel);
                progressWindow.ShowDialog(this);
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
