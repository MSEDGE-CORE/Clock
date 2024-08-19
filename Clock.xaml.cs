using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace Clock
{
    public sealed partial class Clock : Page
    {
        DispatcherTimer Timer;

        public Clock()
        {
            this.InitializeComponent();

            Timer = new DispatcherTimer();
            Timer.Interval = new TimeSpan(0, 0, 0, 0, 10);
            Timer.Tick += Timer_Tick;
            Timer.Start();
        }

        private void Timer_Tick(object sender, object e)
        {
            TimeDisplay.Text = DateTime.Now.ToLongTimeString().ToString();
        }

        private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (ActualWidth > 0 && ActualHeight - 32 > 0)
            {
                if ((ActualHeight - 32) / 1.5 > ActualWidth / 6)
                {
                    TimeDisplay.FontSize = ActualWidth * 1.0 / 6;
                }
                else
                {
                    TimeDisplay.FontSize = (ActualHeight - 32) / 1.5;
                }
                if (!(TimeDisplay.FontSize > 0))
                {
                    TimeDisplay.FontSize = 1;
                }
            }
        }
    }
}
