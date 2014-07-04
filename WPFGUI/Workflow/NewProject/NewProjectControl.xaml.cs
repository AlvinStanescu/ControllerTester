using FM4CC.ExecutionEngine;
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

namespace FM4CC.WPFGUI
{
    /// <summary>
    /// Interaction logic for NewProjectControl.xaml
    /// </summary>
    public partial class NewProjectControl : UserControl, IExecutionInstanceWorker
    {
        public NewProjectControl()
        {
            InitializeComponent();
        }

        public bool Process(ExecutionInstance executionInstance)
        {
            if (this.projectNameTextBox.Text == "")
            {
                MessageBox.Show("Please name the test project.", "Name missing", MessageBoxButton.OK, MessageBoxImage.Stop);
                return false;
            }

            executionInstance.Name = this.projectNameTextBox.Text;
            executionInstance.Environment = this.environmentComboBox.Text;
            executionInstance.PutValue("EnvironmentType", this.environmentTypeComboBox.Text);

            return true;
        }
    }
}
