﻿using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace FM4CC.Util
{
    /// <summary>
    /// Interaction logic for ProgressWindow.xaml
    /// </summary>
    public partial class ProgressWindow : MetroWindow
    {
        private const int GWL_STYLE = -16;
        private const int WS_SYSMENU = 0x80000;
        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);
        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        public ProgressWindow(string windowName, string text)
        {
            InitializeComponent();
            this.ProgressTextBlock.Text = text;
            this.Title = windowName;
            this.ResizeMode = System.Windows.ResizeMode.NoResize;
            this.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterOwner;
        }

        public void ShowDialog(Window owner)
        {
            this.Owner = owner;
            this.ShowDialog();
        }

        public void ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            this.ProgressBar.Value = e.ProgressPercentage;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var hwnd = new WindowInteropHelper(this).Handle;
            SetWindowLong(hwnd, GWL_STYLE, GetWindowLong(hwnd, GWL_STYLE) & ~WS_SYSMENU);
        }

    }
}
