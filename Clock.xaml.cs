﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace Clock
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class Clock : Page
    {
        DispatcherTimer Timer;

        public Clock()
        {
            this.InitializeComponent();

            ApplicationView view = ApplicationView.GetForCurrentView();
            if (!(Application.Current as App).isFullScreen)
            {
                BackControl.Visibility = Visibility.Collapsed;
            }
            else
            {
                BackControl.Visibility = Visibility.Visible;

                var CoreTitleBar = CoreApplication.GetCurrentView().TitleBar;
                CoreTitleBar.ExtendViewIntoTitleBar = true;
                ApplicationViewTitleBar TitleBar = ApplicationView.GetForCurrentView().TitleBar;
                TitleBar.ButtonBackgroundColor = Colors.Transparent;
                TitleBar.ButtonInactiveBackgroundColor = Colors.Transparent;
                Window.Current.SetTitleBar(AppTitleBar);
            }

            Timer = new DispatcherTimer();
            Timer.Interval = new TimeSpan(0, 0, 0, 0, 10);
            Timer.Tick += Timer_Tick;
            Timer.Start();
        }

        private async void Back_Click(object sender, RoutedEventArgs e)
        {
            (Application.Current as App).isFullScreen = false;

            ApplicationView view = ApplicationView.GetForCurrentView();
            if (view.IsFullScreenMode)
            {
                view.ExitFullScreenMode();
            }
            if (view.ViewMode == ApplicationViewMode.CompactOverlay)
            {
                await view.TryEnterViewModeAsync(ApplicationViewMode.Default);
            }
            if (Frame.CanGoBack)
            {
                Frame.GoBack();
            }
        }

        private void Timer_Tick(object sender, object e)
        {
            TimeDisplay.Text = DateTime.Now.ToLongTimeString().ToString();
            if(ActualWidth > 0 && ActualHeight > 0)
            {
                if (ActualHeight / 1.5 > ActualWidth / 6)
                {
                    TimeDisplay.FontSize = ActualWidth * 1.0 / 6;
                }
                else
                {
                    TimeDisplay.FontSize = ActualHeight / 1.5;
                }
                if(!(TimeDisplay.FontSize > 0))
                {
                    TimeDisplay.FontSize = 1;
                }
            }
        }
    }
}
