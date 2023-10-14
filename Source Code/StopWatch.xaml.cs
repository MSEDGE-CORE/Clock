using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.ViewManagement;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.ApplicationModel.Core;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace Clock
{
	/// <summary>
	/// 可用于自身或导航至 Frame 内部的空白页。
	/// </summary>
	public sealed partial class StopWatch : Page
	{
		DispatcherTimer Timer;

        int ThemeSelected = 0;

        public StopWatch()
		{
			this.InitializeComponent();

			TimeDisplay.Text = "";

            ApplicationDataContainer LocalSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            Windows.Storage.ApplicationDataCompositeValue App_Theme = (ApplicationDataCompositeValue)LocalSettings.Values["App_Theme"];
            if (App_Theme != null)
            {
                ThemeSelected = (int)App_Theme["App_Theme"];
            }

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
			Timer.Interval = new TimeSpan(0, 0, 0, 0, 5);
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

        private void StopWatch_Start(object sender, RoutedEventArgs e)
		{
			if((Application.Current as App).StopWatch_IsStart == 0)
            {
                StopWatchStartButtonIcon.Glyph = "\uF8AE";
                StopWatchStopButton.IsEnabled = true;
                StopWatchFlagButton.IsEnabled = true;
                (Application.Current as App).StopWatch_IsStart = 1;
                (Application.Current as App).StopWatch_StartTick = DateTime.Now.Ticks;
			}
			else if((Application.Current as App).StopWatch_IsStart == 1)
            {
                StopWatchStartButtonIcon.Glyph = "\uF5B0";
                StopWatchStopButton.IsEnabled = true;
                StopWatchFlagButton.IsEnabled = false;
                (Application.Current as App).StopWatch_IsStart = 2;
                (Application.Current as App).StopWatch_PauseTick = DateTime.Now.Ticks;
			}
			else if ((Application.Current as App).StopWatch_IsStart == 2)
            {
                StopWatchStartButtonIcon.Glyph = "\uF8AE";
                StopWatchStopButton.IsEnabled = true;
                StopWatchFlagButton.IsEnabled = true;
                (Application.Current as App).StopWatch_IsStart = 1;
                (Application.Current as App).StopWatch_StartTick = DateTime.Now.Ticks - ((Application.Current as App).StopWatch_PauseTick - (Application.Current as App).StopWatch_StartTick);
                (Application.Current as App).StopWatch_PauseTick = 0;
			}

            ApplicationDataContainer LocalSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            Windows.Storage.ApplicationDataCompositeValue StopWatch_Status = new Windows.Storage.ApplicationDataCompositeValue();
			StopWatch_Status["IsStart"] = (Application.Current as App).StopWatch_IsStart;
			StopWatch_Status["StartTick"] = (Application.Current as App).StopWatch_StartTick;
			StopWatch_Status["PauseTick"] = (Application.Current as App).StopWatch_PauseTick;
            StopWatch_Status["Flags"] = (Application.Current as App).StopWatch_Flags;
            LocalSettings.Values["StopWatch_Status"] = StopWatch_Status;
        }

		private void StopWatch_Flag(object sender, RoutedEventArgs e)
		{

            if((Application.Current as App).StopWatch_Flags != "")
                (Application.Current as App).StopWatch_Flags = "\n" + (Application.Current as App).StopWatch_Flags;
            (Application.Current as App).StopWatch_Flags = TimeDisplay.Text + (Application.Current as App).StopWatch_Flags;

            TimeFlag.Text = (Application.Current as App).StopWatch_Flags;

            ApplicationDataContainer LocalSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            Windows.Storage.ApplicationDataCompositeValue StopWatch_Status = new Windows.Storage.ApplicationDataCompositeValue();
            StopWatch_Status["IsStart"] = (Application.Current as App).StopWatch_IsStart;
            StopWatch_Status["StartTick"] = (Application.Current as App).StopWatch_StartTick;
            StopWatch_Status["PauseTick"] = (Application.Current as App).StopWatch_PauseTick;
            StopWatch_Status["Flags"] = (Application.Current as App).StopWatch_Flags;
            LocalSettings.Values["StopWatch_Status"] = StopWatch_Status;
        }

		private void StopWatch_Stop(object sender, RoutedEventArgs e)
        {
            StopWatchStartButtonIcon.Glyph = "\uF5B0";
            StopWatchStopButton.IsEnabled = false;
            StopWatchFlagButton.IsEnabled = false;
            (Application.Current as App).StopWatch_IsStart = 0;
            (Application.Current as App).StopWatch_StartTick = 0;
            (Application.Current as App).StopWatch_PauseTick = 0;
			TimeDisplay.Text = "00:00:00.000";

            (Application.Current as App).StopWatch_Flags = "";

            TimeFlag.Text = (Application.Current as App).StopWatch_Flags;

            ApplicationDataContainer LocalSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            Windows.Storage.ApplicationDataCompositeValue StopWatch_Status = new Windows.Storage.ApplicationDataCompositeValue();
            StopWatch_Status["IsStart"] = (Application.Current as App).StopWatch_IsStart;
            StopWatch_Status["StartTick"] = (Application.Current as App).StopWatch_StartTick;
            StopWatch_Status["PauseTick"] = (Application.Current as App).StopWatch_PauseTick;
            StopWatch_Status["Flags"] = (Application.Current as App).StopWatch_Flags;
            LocalSettings.Values["StopWatch_Status"] = StopWatch_Status;
        }

		private void Timer_Tick(object sender, object e)
		{
            if ((Application.Current as App).StopWatch_IsStart == 1)
            {
                StopWatchStartButtonIcon.Glyph = "\uF8AE";
                StopWatchStopButton.IsEnabled = true;
                StopWatchFlagButton.IsEnabled = true;
            }
            else if ((Application.Current as App).StopWatch_IsStart == 2)
            {
                StopWatchStartButtonIcon.Glyph = "\uF5B0";
                StopWatchStopButton.IsEnabled = true;
                StopWatchFlagButton.IsEnabled = false;
            }
            else if ((Application.Current as App).StopWatch_IsStart == 0)
            {
                StopWatchStartButtonIcon.Glyph = "\uF5B0";
                StopWatchStopButton.IsEnabled = false;
                StopWatchFlagButton.IsEnabled = false;
            }
            TimeFlag.Text = (Application.Current as App).StopWatch_Flags;

            if ((App.Current.RequestedTheme == ApplicationTheme.Light && ThemeSelected == 0) || ThemeSelected == 1)
			{
				SeparateLineDark.StrokeThickness = 0;
				SeparateLineLight.StrokeThickness = 0.5;
			}
			else if ((App.Current.RequestedTheme == ApplicationTheme.Dark && ThemeSelected == 0) || ThemeSelected == 2)
			{
				SeparateLineDark.StrokeThickness = 0.5;
				SeparateLineLight.StrokeThickness = 0;
			}

			if (ActualWidth > 0 && ActualHeight > 0)
            {
                if ((ActualHeight - 64 - 24) / 2 > ActualWidth / 8)
                {
                    BackGrid.Height = (int)(ActualWidth * 1.0 / 8 + 40);
                }
                else
                {
                    BackGrid.Height = (int)((ActualHeight - 64 - 24) / 2 + 40);
                }
                if (!(TimeDisplay.FontSize > 0))
                {
                    TimeDisplay.FontSize = 41;
                }

                if ((ActualHeight - 64 - 24) / 2 > ActualWidth / 8)
				{
					TimeDisplay.FontSize = (int)(ActualWidth * 1.0 / 8);
				}
				else
				{
					TimeDisplay.FontSize = (int)((ActualHeight - 64 - 24) / 2);
                }
                if (!(TimeDisplay.FontSize > 0))
                {
                    TimeDisplay.FontSize = 1;
                }

                SeparateLineLight.Y1 = ActualHeight - 64 - 24;
				SeparateLineLight.Y2 = ActualHeight - 64 - 24;
				SeparateLineDark.Y1 = ActualHeight - 64 - 24;
				SeparateLineDark.Y2 = ActualHeight - 64 - 24;

                Rectangle1.Height = BackGrid.Height;
                StopWatchFlagsViewer.MaxHeight = ActualHeight - 64 - 24 - Rectangle1.Height;
                StopWatchFlagsViewer.MinHeight = ActualHeight - 64 - 24 - Rectangle1.Height;
            }

			if ((Application.Current as App).StopWatch_IsStart == 0)
			{
				TimeDisplay.Text = "00:00:00.000";
			}
			else if ((Application.Current as App).StopWatch_IsStart == 1)
			{
				long NowTicks = (DateTime.Now.Ticks - (Application.Current as App).StopWatch_StartTick) / 10000;

				int Hour = (int)(NowTicks / 1000 / 60 / 60);
				int Minute = (int)((NowTicks % (1000 * 60 * 60)) / 1000 / 60);
				int Second = (int)((NowTicks % (1000 * 60)) / 1000);
				int Msecond = (int)(NowTicks % 1000);

				string DisplayText = "";

				while(Hour >= 100)
				{
					Hour %= 100;
				}
				if(Hour >= 10 && Hour <= 99)
				{
					DisplayText += (Hour / 10).ToString();
					DisplayText += (Hour % 10).ToString();
				}
				else if(Hour >= 0 && Hour <= 9)
                {
                    DisplayText += '0';
                    DisplayText += (Hour % 10).ToString();
				}
				DisplayText += ':';
				if(Minute >= 10 && Minute <= 59)
				{
					DisplayText += (Minute / 10).ToString();
					DisplayText += (Minute % 10).ToString();
				}
				else if(Minute >= 0 && Minute <= 9)
				{
					DisplayText += '0';
					DisplayText += (Minute % 10).ToString();
				}
				DisplayText += ':';
                if (Second >= 10 && Second <= 59)
                {
                    DisplayText += (Second / 10).ToString();
                    DisplayText += (Second % 10).ToString();
                }
                else if (Second >= 0 && Second <= 9)
                {
                    DisplayText += '0';
                    DisplayText += (Second % 10).ToString();
                }
				DisplayText += '.';
                if (Msecond >= 100 && Msecond <= 999)
                {
					DisplayText += (Msecond / 100).ToString();
                    DisplayText += ((Msecond % 100) / 10).ToString();
                    DisplayText += (Msecond % 10).ToString();
                }
                else if (Msecond >= 10 && Msecond <= 99)
                {
                    DisplayText += '0';
                    DisplayText += ((Msecond % 100) / 10).ToString();
                    DisplayText += (Msecond % 10).ToString();
                }
                else if (Msecond >= 0 && Msecond <= 9)
                {
                    DisplayText += '0';
                    DisplayText += '0';
                    DisplayText += (Msecond % 10).ToString();
                }

				TimeDisplay.Text = DisplayText.ToString();
            }
			else if ((Application.Current as App).StopWatch_IsStart == 2)
			{
				long NowTicks = ((Application.Current as App).StopWatch_PauseTick - (Application.Current as App).StopWatch_StartTick) / 10000;

                int Hour = (int)(NowTicks / 1000 / 60 / 60);
                int Minute = (int)((NowTicks % (1000 * 60 * 60)) / 1000 / 60);
                int Second = (int)((NowTicks % (1000 * 60)) / 1000);
                int Msecond = (int)(NowTicks % 1000);

                string DisplayText = "";

                while (Hour >= 100)
                {
                    Hour %= 100;
                }
                if (Hour >= 10 && Hour <= 99)
                {
                    DisplayText += (Hour / 10).ToString();
                    DisplayText += (Hour % 10).ToString();
                }
                else if (Hour >= 0 && Hour <= 9)
                {
                    DisplayText += '0';
                    DisplayText += (Hour % 10).ToString();
                }
                DisplayText += ':';
                if (Minute >= 10 && Minute <= 59)
                {
                    DisplayText += (Minute / 10).ToString();
                    DisplayText += (Minute % 10).ToString();
                }
                else if (Minute >= 0 && Minute <= 9)
                {
                    DisplayText += '0';
                    DisplayText += (Minute % 10).ToString();
                }
                DisplayText += ':';
                if (Second >= 10 && Second <= 59)
                {
                    DisplayText += (Second / 10).ToString();
                    DisplayText += (Second % 10).ToString();
                }
                else if (Second >= 0 && Second <= 9)
                {
                    DisplayText += '0';
                    DisplayText += (Second % 10).ToString();
                }
                DisplayText += '.';
                if (Msecond >= 100 && Msecond <= 999)
                {
                    DisplayText += (Msecond / 100).ToString();
                    DisplayText += ((Msecond % 100) / 10).ToString();
                    DisplayText += (Msecond % 10).ToString();
                }
                else if (Msecond >= 10 && Msecond <= 99)
                {
                    DisplayText += '0';
                    DisplayText += ((Msecond % 100) / 10).ToString();
                    DisplayText += (Msecond % 10).ToString();
                }
                else if (Msecond >= 0 && Msecond <= 9)
                {
                    DisplayText += '0';
                    DisplayText += '0';
                    DisplayText += (Msecond % 10).ToString();
                }

                TimeDisplay.Text = DisplayText;
            }
		}
	}
}
