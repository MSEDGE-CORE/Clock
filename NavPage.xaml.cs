using Microsoft.Toolkit.Uwp.Notifications;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.ViewManagement;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

namespace Clock
{
    public sealed partial class NavPage : Page
    {
        public DispatcherTimer Timer1;

        public NavPage()
        {
            this.InitializeComponent(); 
            var CoreTitleBar = CoreApplication.GetCurrentView().TitleBar;
            CoreTitleBar.ExtendViewIntoTitleBar = true;
            ApplicationViewTitleBar TitleBar = ApplicationView.GetForCurrentView().TitleBar;
            TitleBar.ButtonBackgroundColor = Colors.Transparent;
            TitleBar.ButtonInactiveBackgroundColor = Colors.Transparent;
            Window.Current.SetTitleBar(AppTitleBar);

            ContentFrame.Navigate(typeof(Clock));
            NavClockButton.IsChecked = true;
            NavStopWatchButton.IsChecked = false;
            NavTimerButton.IsChecked = false;
            NavFocusButton.IsChecked = false;
            NavSettingsButton.IsChecked = false;

            Timer1 = new DispatcherTimer();
            Timer1.Interval = new TimeSpan(0, 0, 0, 0, 100);
            Timer1.Tick += Timer_Tick;
            Timer1.Start();
        }

        private void Timer_Tick(object sender, object e)
        {
            if ((Application.Current as App).Timer_IsStart == 1)
            {
                long NowTicks = ((Application.Current as App).Timer_StartTick - DateTime.Now.Ticks / 10000);

                if (NowTicks <= 0)
                {
                    NowTicks = 0;
                    TimerIsUp();
                }
            }
        }

        public async void TimerIsUp()
        {
            new ToastContentBuilder()
                .AddArgument("Action", "Timer")
                .AddText("计时器 时间到")
                .AddText(DateTime.Now.ToLongTimeString().ToString())
                .Show();

            var TimerPage = new Timer();
            TimerPage.TimerStopButton_Click();
            ContentDialog Dialog = new ContentDialog();
            Dialog.XamlRoot = this.XamlRoot;
            Dialog.Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style;
            Dialog.Title = "计时器 时间到";
            Dialog.Content = DateTime.Now.ToLongTimeString().ToString();
            Dialog.CloseButtonText = "好";
            Dialog.DefaultButton = ContentDialogButton.Close;
            var result = await Dialog.ShowAsync();
        }

        private void NavClockButton_Click(object sender, RoutedEventArgs e)
        {
            if (ContentFrame.CurrentSourcePageType != typeof(Clock))
            {
                ContentFrame.Navigate(typeof(Clock), null, new EntranceNavigationTransitionInfo());
            }
            NavClockButton.IsChecked = true;
            NavStopWatchButton.IsChecked = false;
            NavTimerButton.IsChecked = false;
            NavFocusButton.IsChecked = false;
            NavSettingsButton.IsChecked = false;
        }

        private void NavStopWatchButton_Click(object sender, RoutedEventArgs e)
        {
            if (ContentFrame.CurrentSourcePageType != typeof(StopWatch))
            {
                ContentFrame.Navigate(typeof(StopWatch), null, new EntranceNavigationTransitionInfo());
            }
            NavClockButton.IsChecked = false;
            NavStopWatchButton.IsChecked = true;
            NavTimerButton.IsChecked = false;
            NavFocusButton.IsChecked = false;
            NavSettingsButton.IsChecked = false;
        }

        private void NavTimerButton_Click(object sender, RoutedEventArgs e)
        {
            if (ContentFrame.CurrentSourcePageType != typeof(Timer))
            {
                ContentFrame.Navigate(typeof(Timer), null, new EntranceNavigationTransitionInfo());
            }
            NavClockButton.IsChecked = false;
            NavStopWatchButton.IsChecked = false;
            NavTimerButton.IsChecked = true;
            NavFocusButton.IsChecked = false;
            NavSettingsButton.IsChecked = false;
        }

        private void NavSettingsButton_Click(object sender, RoutedEventArgs e)
        {
            if (ContentFrame.CurrentSourcePageType != typeof(Settings))
            {
                ContentFrame.Navigate(typeof(Settings), null, new EntranceNavigationTransitionInfo());
            }
            NavClockButton.IsChecked = false;
            NavStopWatchButton.IsChecked = false;
            NavFocusButton.IsChecked = false;
            NavTimerButton.IsChecked = false;
            NavSettingsButton.IsChecked = true;
        }

        private void NavFocusButton_Click(object sender, RoutedEventArgs e)
        {
            if (ContentFrame.CurrentSourcePageType != typeof(Focus))
            {
                ContentFrame.Navigate(typeof(Focus), null, new EntranceNavigationTransitionInfo());
            }
            NavClockButton.IsChecked = false;
            NavStopWatchButton.IsChecked = false;
            NavFocusButton.IsChecked = false;
            NavTimerButton.IsChecked = true;
            NavSettingsButton.IsChecked = false;
        }
    }
}
