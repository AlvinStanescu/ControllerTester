using FM4CC.ExecutionEngine;
using FM4CC.Simulation;
using FM4CC.WPFGUI.Configuration;
using FM4CC.WPFGUI.Simulation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;

namespace FM4CC.WPFGUI
{
    internal class NewProjectViewModel : INotifyPropertyChanged
    {
        public ExecutionEnvironment SelectedEnvironment { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;
        public ExecutionInstance ExecutionInstance { get { return executionInstance; } }
        public SimulationSettings SimulationSettings 
        { 
            get 
            {
                if (simulationSettingsControl != null)
                {
                    return simulationSettingsControl.ModelSimulationSettings;
                }
                else
                {
                    return null;
                }
            } 
        }
        public CollectionView ExecutionEnvironmentTypes
        {
            get { return new CollectionView(SelectedEnvironment.SystemTypes); }
        }

        public CollectionView ExecutionEnvironments
        {
            get { return executionEnvironments; }
        }

        private readonly CollectionView executionEnvironments;
        private ExecutionInstance executionInstance;
        private UserControl currentView;
        private SimulationSettingsControl simulationSettingsControl;
        private StackPanel controlContainer;
        private FMTesterConfiguration appConfiguration;

        internal NewProjectViewModel(ICollection<ExecutionEnvironment> availableExecutionEngines, ref ExecutionInstance executionInstance, FMTesterConfiguration configuration, StackPanel controlContainer)
        {
            executionEnvironments = new CollectionView(availableExecutionEngines);
            executionInstance = new ExecutionInstance();

            this.simulationSettingsControl = null;
            this.executionInstance = executionInstance;
            this.appConfiguration = configuration;
            SelectedEnvironment = availableExecutionEngines.First();

            this.controlContainer = controlContainer;
            this.currentView = new NewProjectControl();
            this.currentView.DataContext = this;
            controlContainer.Children.Add(currentView);

            IsDone = false;
        }

        internal void ShowNext(object sender, NewProjectWindow window)
        {
            bool success = (currentView as IExecutionInstanceWorker).Process(executionInstance);

            if (!success) return;

            if (currentView.GetType() == typeof(NewProjectControl))
            {
                // TODO not working if view is in another assembly, to be added
                controlContainer.Children.Remove(currentView);
                currentView = (UserControl)Activator.CreateInstance(Type.GetType("FM4CC.WPFGUI.ExecutionEngine." + executionInstance.Environment + "ConfigurationControl", true, true));
                controlContainer.Children.Add(currentView);
                currentView.DataContext = this;
                (sender as Button).Content = "Next";
            }
            else
            {
                (sender as Button).Visibility = System.Windows.Visibility.Collapsed;
                window.controlContainer.SetBinding(Grid.RowSpanProperty, "2");
                IsDone = true;

                controlContainer.Children.Remove(currentView);
                simulationSettingsControl = new SimulationSettingsControl(window, null, appConfiguration, executionInstance, SelectedEnvironment);
                controlContainer.Children.Add(simulationSettingsControl);
            }
        }

        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }


        public bool IsDone { get; set; }
    }
}
