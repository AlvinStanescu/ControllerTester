using FM4CC.ExecutionEngine;
using FM4CC.ExecutionEngine.Matlab;
using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Controls.Ribbon;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FM4CC.WPFGUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        private MainViewModel viewModel;

        public MainWindow()
        {
            InitializeComponent();
            Ribbon.ShowQuickAccessToolBarOnTop = false;

            viewModel = new MainViewModel(this);
        }

        #region Ribbon Creation
        private void Ribbon_LayoutUpdated(object sender, EventArgs e)
        {
            // Hack to hide the quick access toolbar of the ribbon, which we do not use
            // The special MetroWindow controls are used in place
            RectangleGeometry clipRectangle = new RectangleGeometry();
            clipRectangle.Rect = new Rect(0, 20, Ribbon.ActualWidth, Ribbon.ActualHeight - 20);
            Ribbon.Clip = clipRectangle;

        }

        /// <summary>
        /// Replaces the image and down arrow of a Ribbon Application Menu Button with the button's Label text.
        /// </summary>
        /// <param name="menu">The menu whose application button should show the label text.</param>
        /// <remarks>
        /// The method assumes the specific visual tree implementation of the October 2010 version of <see cref="RibbonApplicationMenu "/>.
        /// Fortunately, since the application menu is high profile, breakage due to an version changes should be obvious.
        /// Hopefully, the native support for text will be added before the implementation breaks.
        /// </remarks>
        void ReplaceRibbonApplicationMenuButtonContent(RibbonApplicationMenu menu)
        {
            Grid outerGrid = (Grid)VisualTreeHelper.GetChild(menu, 0);
            RibbonToggleButton toggleButton = (RibbonToggleButton)outerGrid.Children[0];
            toggleButton.Foreground = Brushes.Gray;

            ReplaceRibbonToggleButtonContent(toggleButton, menu.Label);

            Popup popup = (Popup)outerGrid.Children[2];
            EventHandler popupOpenedHandler = null;
            popupOpenedHandler = new EventHandler(delegate
            {
                Decorator decorator = (Decorator)popup.Child;
                Grid popupGrid = (Grid)decorator.Child;
                Canvas canvas = (Canvas)popupGrid.Children[1];
                RibbonToggleButton popupToggleButton = (RibbonToggleButton)canvas.Children[0];
                toggleButton.Foreground = Brushes.Gray;

                ReplaceRibbonToggleButtonContent(popupToggleButton, menu.Label);
                popup.Opened -= popupOpenedHandler;
            });
            popup.Opened += popupOpenedHandler;
        }

        void ReplaceRibbonToggleButtonContent(RibbonToggleButton toggleButton, string text)
        {
            // Subdues the aero highlighting to that the text has better contrast.
            Grid grid = (Grid)VisualTreeHelper.GetChild(toggleButton, 0);
            Border middleBorder = (Border)grid.Children[1];
            middleBorder.Opacity = 0;

            // Replaces the images with the label text.
            StackPanel stackPanel = (StackPanel)grid.Children[2];
            UIElementCollection children = stackPanel.Children;
            children.RemoveRange(0, children.Count);
            TextBlock textBlock = new TextBlock(new Run(text));
            textBlock.Foreground = Brushes.White;
            textBlock.VerticalAlignment = System.Windows.VerticalAlignment.Bottom;
            children.Add(textBlock);
        }
        #endregion

        #region Callback Methods
        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            ReplaceRibbonApplicationMenuButtonContent(Ribbon.ApplicationMenu);
            if (AppDomain.CurrentDomain.SetupInformation.ActivationArguments != null && AppDomain.CurrentDomain.SetupInformation.ActivationArguments.ActivationData != null && AppDomain.CurrentDomain.SetupInformation.ActivationArguments.ActivationData.Any())
            {
                string[] activationData = AppDomain.CurrentDomain.SetupInformation.ActivationArguments.ActivationData;
                var uri = new Uri(activationData[0]);
                viewModel.ShowOpenProject(uri.LocalPath);
            }
        }

        private void NewTestProject_Click(object sender, RoutedEventArgs e)
        {
            viewModel.ShowNewProject();
        }

        private void OpenTestProject_Click(object sender, RoutedEventArgs e)
        {
            viewModel.ShowOpenProject();
        }

        private void SaveTestProject_Click(object sender, RoutedEventArgs e)
        {
            viewModel.ShowSaveProject();
        }

        private void SaveTestProjectAs_Click(object sender, RoutedEventArgs e)
        {
            viewModel.ShowSaveProjectAs();
        }

        private void Settings_Click(object sender, RoutedEventArgs e)
        {
            viewModel.ShowSettingsWindow();
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Generate_Clicked(object sender, RoutedEventArgs e)
        {
            this.viewModel.PerformGeneration();
        }

        private void Clean_Clicked(object sender, RoutedEventArgs e)
        {
            this.viewModel.CleanTestCases();
        }

        private void Execute_Clicked(object sender, RoutedEventArgs e)
        {
            this.viewModel.ExecuteTestCase(ComboBoxTestCases.SelectionBoxItem);
        }

        private void Simulate_Clicked(object sender, RoutedEventArgs e)
        {
            this.viewModel.RunSUT();
        }

        private void RunMATLAB_Click(object sender, RoutedEventArgs e)
        {
            this.viewModel.RunProgram("MATLAB");
        }

        private void CloseTestProject_Click(object sender, RoutedEventArgs e)
        {
            viewModel.CloseProject();
        }

        private void cbTestCases_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            viewModel.ChangedSelectedTestCase(e);
        }

        private void ShowConsole_Click(object sender, RoutedEventArgs e)
        {
            // TODO
        }

        private void FaultModelsListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            viewModel.SelectedFaultModel(e);
        }

        private void ChangeSimulationSettings_Clicked(object sender, RoutedEventArgs e)
        {
            viewModel.ChangeSimulationSettings();
        }
        #endregion

        private void About_Click(object sender, RoutedEventArgs e)
        {
            new AboutBox(this).ShowDialog(); 
        }

        private void MetroWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            MessageBoxResult question = MessageBox.Show("Do you want to save the current project upon exiting?", "Controller Tester", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
            
            if (question == MessageBoxResult.Yes)
            {
                viewModel.ShowSaveProject();
            }
            else if (question == MessageBoxResult.Cancel)
            {
                e.Cancel = true;
            }
        }
    }
}
