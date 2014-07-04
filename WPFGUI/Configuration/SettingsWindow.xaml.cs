using MahApps.Metro.Controls;
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
using System.Windows.Shapes;

namespace FM4CC.WPFGUI.Configuration
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    internal partial class SettingsWindow : MetroWindow
    {
        private FMTesterConfiguration config;

        internal SettingsWindow(FMTesterConfiguration configuration)
        {
            InitializeComponent();
            this.config = configuration;
            this.MatlabPathTextBox.Text = config.MatLABFolderPath;
            this.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterOwner;

        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (!Directory.Exists(this.MatlabPathTextBox.Text) || !File.Exists(this.MatlabPathTextBox.Text + "\\bin\\matlab.exe"))
            {
                MessageBox.Show("The specified path for MATLAB is invalid.");
            }
            else
            {
                config.MatLABFolderPath = this.MatlabPathTextBox.Text;
                config.SaveSettings(Constants.SettingsFilePath);
                this.Close();
            }
        }


        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        public void ShowDialog(Window owner)
        {
            this.Owner = owner;
            this.ShowDialog();
        }

        private void Browse_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog fbd = new System.Windows.Forms.FolderBrowserDialog();
            fbd.ShowDialog();
            this.MatlabPathTextBox.Text = fbd.SelectedPath;
        }
    }
}
