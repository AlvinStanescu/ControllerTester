using FM4CC.ExecutionEngine;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace FM4CC.WPFGUI.ExecutionEngine
{
    /// <summary>
    /// Interaction logic for MatLabConfigurationControl.xaml
    /// </summary>
    public partial class MatLabConfigurationControl : UserControl, IExecutionInstanceWorker
    {
        public MatLabConfigurationControl()
        {
            InitializeComponent();
        }

        public bool Process(ExecutionInstance executionInstance)
        {
            if ((!ModelPathTextBox.Text.Contains(".mdl")  && !ModelPathTextBox.Text.Contains(".slx")) || !File.Exists(this.ModelPathTextBox.Text))
            {
                MessageBox.Show("Invalid model path, expected a MATLAB model file (.mdl, .slx)", "Model missing", MessageBoxButton.OK, MessageBoxImage.Stop);
                return false;
            }

            if (!ModelSettingsPathTextBox.Text.Contains(".m") || !File.Exists(this.ModelSettingsPathTextBox.Text))
            {
                MessageBox.Show("Invalid model settings path, expected a MATLAB script file (.m)", "Model settings missing", MessageBoxButton.OK, MessageBoxImage.Stop);
                return false;
            }
            
            executionInstance.PutValue("SUTSettingsPath", this.ModelSettingsPathTextBox.Text);
            executionInstance.PutValue("SUTPath", this.ModelPathTextBox.Text);

            return true;
        }

        private void ModelBrowse_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fbd = new OpenFileDialog();
            fbd.ShowDialog();
            this.ModelPathTextBox.Text = fbd.FileName;
        }

        private void ModelSettingsBrowse_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fbd = new OpenFileDialog();
            fbd.ShowDialog();
            this.ModelSettingsPathTextBox.Text = fbd.FileName;
        }

    }
}
