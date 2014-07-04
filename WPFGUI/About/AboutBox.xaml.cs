using MahApps.Metro.Controls;
using System.Diagnostics;
using System.Windows;

namespace FM4CC.WPFGUI
{
    /// <summary>
    /// Interaction logic for AboutBoxSimple.xaml
    /// </summary>
    public partial class AboutBox : MetroWindow
    {
        private AboutAssemblyDataProvider aboutDataProvider = new AboutAssemblyDataProvider();

        protected AboutBox()
        {
            InitializeComponent();
            DisplayPanel.DataContext = aboutDataProvider;
            this.WindowTransitionsEnabled = false;
            this.EnableDWMDropShadow = true;

        }

        public AboutBox(Window parent)
            : this()
        {
            this.Owner = parent;
        }

        private void hyperlink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            if (e.Uri != null && string.IsNullOrEmpty(e.Uri.OriginalString) == false)
            {
                string uri = e.Uri.AbsoluteUri;
                Process.Start(new ProcessStartInfo(uri));

                e.Handled = true;
            }
        }
    }
}
