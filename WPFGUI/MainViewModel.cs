using FM4CC.Environment;
using FM4CC.ExecutionEngine;
using FM4CC.ExecutionEngine.Matlab;
using FM4CC.FaultModels;
using FM4CC.Simulation;
using FM4CC.TestCase;
using FM4CC.Util;
using FM4CC.WPFGUI.Configuration;
using FM4CC.WPFGUI.Project;
using FM4CC.WPFGUI.Simulation;
using FM4CC.WPFGUI.Workflow.OpenSave;
using FM4CC.WPFGUI.Workflow.Workers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Xml;
using System.Xml.Serialization;

namespace FM4CC.WPFGUI
{
    internal class MainViewModel : INotifyPropertyChanged
    {
        private TestProject currentTestProject = null;
        private FMTesterConfiguration appConfiguration = null;
        private MainWindow mainWindow;

        private IDictionary<string, Assembly> faultModelAssemblies;
        private IDictionary<string, FaultModel> faultModels;

        private IDictionary<ListViewItem, UserControl> connectedControl;
        private IDictionary<FaultModel, ListViewItem> connectedListViewItem;

        private IList<ExecutionEnvironment> selectedExecutionEngines;
        private IDictionary<string, ExecutionEnvironment> allExecutionEngines;

        private ProgressWindow progressWindow;

        internal MainViewModel(MainWindow mainWindow)
        {
            this.mainWindow = mainWindow;
            this.mainWindow.WindowState = WindowState.Maximized;
            LoadAssemblies();
            LoadExecutionEngines(new List<string>() {Directory.GetCurrentDirectory()});
            connectedControl = new Dictionary<ListViewItem, UserControl>();
            connectedListViewItem = new Dictionary<FaultModel, ListViewItem>();

            mainWindow.Closing += mainWindow_Closing;
            mainWindow.Loaded += mainWindow_Loaded;
            selectedExecutionEngines = new List<ExecutionEnvironment>();
            
        }

        void mainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            appConfiguration = FMTesterConfiguration.LoadSettings(Constants.SettingsFilePath);
            if (appConfiguration == null)
            {
                ShowSettingsWindow(false);
                appConfiguration = FMTesterConfiguration.LoadSettings(Constants.SettingsFilePath);
            }   
        }

        private void LoadAssemblies()
        {
            faultModelAssemblies = new Dictionary<string, Assembly>();
            string currentDir = Directory.GetCurrentDirectory();
            string[] dllFiles = Directory.GetFiles(currentDir, "*FaultModel.dll", SearchOption.AllDirectories);

            foreach (string assemblyPath in dllFiles)
            {
                string assembly = Path.GetFileNameWithoutExtension(assemblyPath);
                faultModelAssemblies.Add(assembly, Assembly.Load(assembly));
            }
        }

        void mainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (selectedExecutionEngines == null) return;

            foreach (ExecutionEnvironment ev in selectedExecutionEngines)
            {
                ev.Dispose();
            }
        }

        internal void ShowNewProject()
        {
            ExecutionInstance executionInstance = null;
            NewProjectWindow newProject = new NewProjectWindow(allExecutionEngines.Values, ref executionInstance, appConfiguration);
            newProject.Owner = mainWindow;
            if ((bool)newProject.ShowDialog())
            {
                currentTestProject = new TestProject(executionInstance, newProject.SimulationSettings);
                InitializeProject();
            }
        }

        private void InitializeProject()
        {
            mainWindow.Title += " - " + currentTestProject.Name;
            
            // only a single engine per test project is supported currently
            selectedExecutionEngines.Clear();
            selectedExecutionEngines.Add(allExecutionEngines[currentTestProject.Instance.Environment.ToLower()]);

            LoadFaultModels(new List<string>() { Directory.GetCurrentDirectory() }, selectedExecutionEngines[0]);
            VisualizeFaultModels();

            mainWindow.SimulationGroup.IsEnabled = true;
            mainWindow.TestCaseGenerationGroup.IsEnabled = true;
            mainWindow.CloseProjectMenuItem.IsEnabled = true;
            mainWindow.SaveProjectMenuItem.IsEnabled = true;
        }

        /// <summary>
        /// Loads the execution environment assemblies from the list of paths
        /// </summary>
        /// <param name="executionEnvironmentPaths">List of the paths to all of the ExecutionEnvironment assemblies</param>
        private void LoadExecutionEngines(IList<string> executionEnvironmentPaths)
        {
            // Mock
            // read execution environment assemblies from XML or directory
            // load execution environment assemblies
            allExecutionEngines = new Dictionary<string, ExecutionEnvironment>();
            ExecutionEnvironment e = new MatlabExecutionEngine();
            allExecutionEngines.Add(e.Name.ToLower(), e);

        }
        /// <summary>
        /// Loads all fault models present in the path for a specific execution environment (e.g. MATLAB)
        /// </summary>
        /// <param name="faultModelPaths">List of the paths to all of the FaultModel assemblies</param>
        /// <param name="environment">The chosen execution environment for the current test project</param>
        private void LoadFaultModels(IList<string> faultModelPaths, ExecutionEnvironment environment)
        {
            faultModels = new Dictionary<string, FaultModel>();

            // load fault models from assemblies
            // initialize the used ones with the proper configuration from the project file, if any

            if (currentTestProject.FaultModelSettings != null)
            {
                foreach (string faultModelName in faultModelAssemblies.Keys)
                {
                    if (currentTestProject.FaultModelSettings.ContainsKey(faultModelName))
                    {
                        object[] args = { environment, currentTestProject.FaultModelSettings[faultModelName], appConfiguration.ScriptsPath };
                        try
                        {
                            FaultModel newFaultModel = (FaultModel)Activator.CreateInstance(faultModelAssemblies[faultModelName].GetType("FM4CC.FaultModels." + faultModelName.Substring(0, faultModelName.IndexOf("FaultModel")) + "." + faultModelName), args);
                            faultModels.Add(faultModelName, newFaultModel);
                        }
                        catch (FM4CCException)
                        {
                            // fault model does not support the chosen execution environment or SUT type (should, however, not happen in this case)
                        }
                    }
                    else
                    {
                        object[] args = { environment, (FaultModelConfiguration)Activator.CreateInstance(faultModelAssemblies[faultModelName].GetType("FM4CC.FaultModels." + faultModelName.Substring(0, faultModelName.IndexOf("FaultModel")) + "." + faultModelName + "Configuration")), appConfiguration.ScriptsPath };
                        try
                        {
                            FaultModel newFaultModel = (FaultModel)Activator.CreateInstance(faultModelAssemblies[faultModelName].GetType("FM4CC.FaultModels." + faultModelName.Substring(0, faultModelName.IndexOf("FaultModel")) + "." + faultModelName), args);
                            faultModels.Add(faultModelName, newFaultModel);
                        }
                        catch (FM4CCException)
                        {
                            // fault model does not support the chosen execution environment or SUT type, skip
                        }
                    }
                }
            }
            else
            {
                foreach (string faultModelName in faultModelAssemblies.Keys)
                {
                    FaultModelConfiguration fmConfig = (FaultModelConfiguration)Activator.CreateInstance(faultModelAssemblies[faultModelName].GetType("FM4CC.FaultModels." + faultModelName.Substring(0, faultModelName.IndexOf("FaultModel")) + "." + faultModelName + "Configuration"));

                    object[] args = { environment, fmConfig, appConfiguration.ScriptsPath };
                    try
                    {
                        faultModels.Add(faultModelName, (FaultModel)Activator.CreateInstance(faultModelAssemblies[faultModelName].GetType("FM4CC.FaultModels." + faultModelName.Substring(0, faultModelName.IndexOf("FaultModel")) + "." + faultModelName), args));
                        currentTestProject.FaultModelSettings = new SerializableDictionary<string, FaultModelConfiguration>();
                        currentTestProject.FaultModelSettings.Add(faultModelName, fmConfig);
                    }
                    catch (FM4CCException)
                    {
                        // fault model does not support the chosen execution environment or SUT type, skip
                    }
                }
            }
        }

        private FaultModel GetLinkedFaultModelForCheckBox(CheckBox cb)
        {
            BindingExpression fmBinding = cb.GetBindingExpression(CheckBox.ContentProperty) as BindingExpression;
            return (FaultModel)(fmBinding.DataItem);
        }

        private void VisualizeFaultModels()
        {
            mainWindow.FaultModelPlaceholderTextBlock.Visibility = Visibility.Collapsed;
            mainWindow.FaultModelsListView.Visibility = Visibility.Visible;
            mainWindow.FaultModelsStackPanel.VerticalAlignment = VerticalAlignment.Top;
            mainWindow.ConfigurationPlaceholderTextBlock.Visibility = Visibility.Collapsed;
            mainWindow.ConfigurationStackPanel.VerticalAlignment = VerticalAlignment.Top;

            foreach (FaultModel fm in faultModels.Values)
            {
                string faultModelName = fm.ToString();
                object[] args = {fm.FaultModelConfiguration};
                                
                UserControl faultModelConfiguration = (UserControl)Activator.CreateInstance(faultModelAssemblies[faultModelName].GetType("FM4CC.FaultModels." + faultModelName.Substring(0, faultModelName.IndexOf("FaultModel")) + ".GUI.ConfigurationControl"), args);
                                
                ListViewItem item = new ListViewItem();
                item.Uid = fm.ToString();
                connectedControl[item] = faultModelConfiguration;
                connectedListViewItem[fm] = item;

                TextBlock itemText = new TextBlock();
                itemText.VerticalAlignment = VerticalAlignment.Center;
                itemText.HorizontalAlignment = HorizontalAlignment.Center;
                item.Content = itemText;

                Binding nameBinding = new Binding("Name");
                nameBinding.Source = fm;
                nameBinding.Mode = BindingMode.OneWay;
                itemText.SetBinding(TextBlock.TextProperty, nameBinding);

                item.Margin = new System.Windows.Thickness(0.0, 10.0, 0.0, 10.0);

                mainWindow.FaultModelsListView.Items.Add(item);
            }
            mainWindow.FaultModelsListView.SelectedIndex = 0;
        }

        private void RemoveFaultModelsFromView()
        {
            mainWindow.FaultModelsStackPanel.VerticalAlignment = VerticalAlignment.Center;
            mainWindow.ConfigurationPlaceholderTextBlock.Visibility = Visibility.Visible;
            mainWindow.ConfigurationStackPanel.VerticalAlignment = VerticalAlignment.Top;

            connectedListViewItem.Clear();
            connectedControl.Clear();
            mainWindow.FaultModelsListView.Items.Clear();
        }

        #region Generation
        internal void PerformGeneration()
        {
            ListViewItem faultModelListViewItem = mainWindow.FaultModelsListView.SelectedItem as ListViewItem;
            IValidatable configurationTab = connectedControl[faultModelListViewItem] as IValidatable;
            FaultModel fm = faultModels[faultModelListViewItem.Uid];

            if (configurationTab.Validate())
            {
                fm.ExecutionInstance = currentTestProject.Instance;
                fm.SimulationSettings = currentTestProject.ModelSimulationSettings;

                var logType = typeof(Action<>);
                var myClass = typeof(MainViewModel);
                var myMethod = myClass.GetMethod("Log");
                var typeArgs = new[] { typeof(string) };
                var constructed = logType.MakeGenericType(typeArgs);

                var ctrArgs = typeof(Action<>).MakeGenericType(typeArgs);
                var dlg = Delegate.CreateDelegate(ctrArgs, this, myMethod);

                object[] args = { fm, currentTestProject.TestCases, dlg };
                fm.ExecutionInstance = currentTestProject.Instance;
                string fmName = fm.ToString();
                Window testGenerationWindow = (Window)Activator.CreateInstance(faultModelAssemblies[fmName].GetType("FM4CC.FaultModels." + fmName.Substring(0, fmName.IndexOf("FaultModel")) + ".GUI.TestGenerationWindow"), args);
                testGenerationWindow.Owner = this.mainWindow;
                testGenerationWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                
                if ((bool)testGenerationWindow.ShowDialog())
                {
                    mainWindow.TestCaseExecutionGroup.IsEnabled = true;
                    mainWindow.TestCaseInfoGroup.IsEnabled = true;
                    mainWindow.ComboBoxTestCases.ItemsSource = CollectionViewSource.GetDefaultView(currentTestProject.TestCases);
                    mainWindow.ComboBoxTestCases.SelectedItem = currentTestProject.TestCases[0];
                }
            }
            else
            {
                MessageBox.Show("Failed to perform generation for fault model " + fm.Name, "Configuration error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
   
        }

        #endregion

        internal void ShowSaveProject()
        {
            bool runSave = true;
            if (currentTestProject != null)
            {
                foreach (FaultModel fm in faultModels.Values)
                {
                    IValidatable configurationTab = connectedControl[connectedListViewItem[fm]] as IValidatable;
                    if (!configurationTab.Validate())
                    {
                        MessageBox.Show("Could not save the project, please check the fault model configuration for " + fm.Name, "Configuration error", MessageBoxButton.OK, MessageBoxImage.Error);
                        runSave = false;
                        break;
                    }                 
                }

                if (runSave)
                {
                    OpenSaveHandler.SaveProject(currentTestProject);
                }
            }
        }

        internal void ShowOpenProject(string path = null)
        {
            OpenSaveHandler.OpenProjectStatus status = OpenSaveHandler.OpenProject(ref currentTestProject, path);

            if (status == OpenSaveHandler.OpenProjectStatus.Opened)
            {
                CloseProject();
                InitializeProject();

                mainWindow.TestCaseGenerationGroup.IsEnabled = true;                

                if (currentTestProject.TestCases.Count != 0)
                {
                    mainWindow.TestCaseExecutionGroup.IsEnabled = true;
                    mainWindow.TestCaseInfoGroup.IsEnabled = true;
                    mainWindow.ComboBoxTestCases.ItemsSource = CollectionViewSource.GetDefaultView(currentTestProject.TestCases);
                    mainWindow.ComboBoxTestCases.SelectedItem = currentTestProject.TestCases[0];
                }
            }
            else if (status == OpenSaveHandler.OpenProjectStatus.Invalid)
            {
                MessageBox.Show("Cannot open file, format unrecognized.", "Error openin file", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            
        }

        public void Log(string text)
        {
            mainWindow.ConsoleTextBox.Text += text + "\r\n";
        }

        #region Settings

        public void ShowSettingsWindow(bool canDiscard = true)
        {
            SettingsWindow settingsWindow = new SettingsWindow(appConfiguration, canDiscard);
            settingsWindow.ShowDialog(this.mainWindow);
        }

        #endregion

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetField<T>(ref T field, T value, string propertyName)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }
        #endregion




        internal void RunProgram(string program)
        {
            switch (program)
            {
                case "MATLAB":
                    Process.Start(appConfiguration.MatLABFolderPath + "\\bin\\matlab.exe");
                    break;
            }
        }

        internal void CloseProject()
        {
            mainWindow.Title = "Fault Model Tester";

            mainWindow.CloseProjectMenuItem.IsEnabled = false;
            mainWindow.SaveProjectMenuItem.IsEnabled = false;
            
            if (mainWindow.ConfigurationStackPanel.Children.Count > 1)
            {
                mainWindow.ConfigurationStackPanel.Children.RemoveAt(1);
            }

            mainWindow.ConfigurationStackPanel.VerticalAlignment = VerticalAlignment.Center;

            mainWindow.ConfigurationPlaceholderTextBlock.Visibility = Visibility.Visible;
            mainWindow.FaultModelPlaceholderTextBlock.Visibility = Visibility.Visible;

            // only a single engine per test project is supported currently
            selectedExecutionEngines.Clear();

            RemoveFaultModelsFromView();

            mainWindow.SimulationGroup.IsEnabled = false;
            mainWindow.TestCaseGenerationGroup.IsEnabled = false;
            mainWindow.TestCaseExecutionGroup.IsEnabled = false;
            mainWindow.TestCaseInfoGroup.IsEnabled = false;
            mainWindow.TestCaseInfoText.Text = "";
        }

        internal void CleanTestCases()
        {
            currentTestProject.TestCases.Clear();
            mainWindow.ComboBoxTestCases.ItemsSource = null;
            mainWindow.TestCaseExecutionGroup.IsEnabled = false;
            mainWindow.TestCaseInfoGroup.IsEnabled = false;
            mainWindow.TestCaseInfoText.Text = "";
        }

        internal void ExecuteTestCase(object selected)
        {
            FaultModelTesterTestCase selectedTestCase = selected as FaultModelTesterTestCase;
            FaultModel fm = faultModels[selectedTestCase.FaultModel];

            IValidatable configurationTab = connectedControl[connectedListViewItem[fm]] as IValidatable;
            if (!configurationTab.Validate())
            {
                MessageBox.Show("Failed to perform simulation for fault model " + fm.Name, "Configuration error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            
            fm.ExecutionInstance = currentTestProject.Instance;
            TestCaseExecutionWorker worker = new TestCaseExecutionWorker(fm);
            worker.RunWorkerCompleted += testCaseExecutionWorker_RunWorkerCompleted;
            progressWindow = new ProgressWindow("Running the test case", "Please wait while the test case is run...");
            worker.ProgressChanged += progressWindow.ProgressChanged;
            worker.RunWorkerAsync(selectedTestCase);
            progressWindow.ShowDialog(this.mainWindow);

            
        }

        private void testCaseExecutionWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            progressWindow.Close();
        }

        internal void ChangedSelectedTestCase(SelectionChangedEventArgs e)
        {
            if (e.Source != null)
            {
                FaultModelTesterTestCase testCase = mainWindow.ComboBoxTestCases.SelectedItem as FaultModelTesterTestCase;
                if (testCase != null)
                {
                    mainWindow.TestCaseInfoText.Text = ((FaultModelTesterTestCase)mainWindow.ComboBoxTestCases.SelectedItem).GetDescription();
                }
            }
        }

        internal void RunSUT()
        {
            FaultModel fm = faultModels[((ListViewItem)mainWindow.FaultModelsListView.SelectedItem).Uid];
            var logType = typeof(Action<>);
            var myClass = typeof(MainViewModel);
            var myMethod = myClass.GetMethod("Log");
            var typeArgs = new[] { typeof(string) };
            var constructed = logType.MakeGenericType(typeArgs);

            var ctrArgs = typeof(Action<>).MakeGenericType(typeArgs);
            var dlg = Delegate.CreateDelegate(ctrArgs, this, myMethod);

            object[] args = { fm, dlg };
            fm.ExecutionInstance = currentTestProject.Instance;
            fm.SimulationSettings = this.currentTestProject.ModelSimulationSettings;
            string fmName = fm.ToString();
            Window runWindow = (Window)Activator.CreateInstance(faultModelAssemblies[fmName].GetType("FM4CC.FaultModels." + fmName.Substring(0, fmName.IndexOf("FaultModel")) + ".GUI.RunWindow"), args);
            runWindow.Owner = this.mainWindow;

            runWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            runWindow.ShowDialog();
        }

        internal void SelectedFaultModel(SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                ListViewItem item = (ListViewItem)e.AddedItems[0];
                
                if (mainWindow.ConfigurationStackPanel.Children.Count > 1)
                {
                    mainWindow.ConfigurationStackPanel.Children.RemoveAt(1);
                }                
                
                mainWindow.ConfigurationStackPanel.Children.Add(connectedControl[item]);
                mainWindow.FaultModelDescriptionTextBox.Text = faultModels[item.Uid].Description;
            }
        }

        internal void ChangeSimulationSettings()
        {
            SimulationSettingsWindow simulationSettingsWindow = new SimulationSettingsWindow(currentTestProject.ModelSimulationSettings, appConfiguration, currentTestProject.Instance, selectedExecutionEngines[0]);
            simulationSettingsWindow.Owner = mainWindow;
            simulationSettingsWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            simulationSettingsWindow.ShowDialog();

            currentTestProject.ModelSimulationSettings = simulationSettingsWindow.ModelSimulationSettings;
        }
    }
}
