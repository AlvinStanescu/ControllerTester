using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace FM4CC.WPFGUI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private const int minimumSplashScreenDuration = 2000;
        private const int splashScreenFadeTime = 500;  

        protected override void OnStartup(StartupEventArgs e)
        {
            SplashScreen splash = new SplashScreen("Resources\\SplashScreen.png");
            splash.Show(false);
            
            Stopwatch timer = new Stopwatch();
            timer.Start();

            base.OnStartup(e);
            MainWindow main = new MainWindow();

            timer.Stop();
            int remainingTimeToShowSplash = minimumSplashScreenDuration - (int)timer.ElapsedMilliseconds;
            if (remainingTimeToShowSplash > 0)
            {
                Thread.Sleep(remainingTimeToShowSplash);
            }

            splash.Close(TimeSpan.FromMilliseconds(splashScreenFadeTime));
            main.Show();
        }
    }
}
