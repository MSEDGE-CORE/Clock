using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.Devices;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace Clock
{
    public sealed partial class Timer : Page
    {
        DispatcherTimer Timer1;

        public Timer()
        {
            this.InitializeComponent();

            TextHour.Text = (Application.Current as App).Timer_HourSet.ToString();
            TextMinute.Text = (Application.Current as App).Timer_MinuteSet.ToString();
            TextSecond.Text = (Application.Current as App).Timer_SecondSet.ToString();

            Timer1 = new DispatcherTimer();
            Timer1.Interval = new TimeSpan(0, 0, 0, 0, 5);
            Timer1.Tick += Timer_Tick;
            Timer1.Start();
        }

        private void Timer_Tick(object sender, object e)
        {
            if ((Application.Current as App).Timer_IsStart == 1)
            {
                TimerStartButtonIcon.Glyph = "\uF8AE";
                TimerStopButton.IsEnabled = true;
                TimerEditButton.IsEnabled = false;

                if (ActualHeight >= TimeDisplay.FontSize + 56 * 2 + 100 && TextH1.FontSize == 50)
                {
                    OTimerShow.Visibility = Visibility.Visible;
                    ProgressGrid.Visibility = Visibility.Visible;
                }
                else
                {
                    OTimerShow.Visibility = Visibility.Collapsed;
                    ProgressGrid.Visibility = Visibility.Collapsed;
                }
            }
            else if ((Application.Current as App).Timer_IsStart == 2)
            {
                TimerStartButtonIcon.Glyph = "\uF5B0";
                TimerStopButton.IsEnabled = true;
                TimerEditButton.IsEnabled = false;

                if (ActualHeight >= TimeDisplay.FontSize + 56 * 2 + 100 && TextH1.FontSize == 50)
                {
                    OTimerShow.Visibility = Visibility.Visible;
                    ProgressGrid.Visibility = Visibility.Visible;
                }
                else
                {
                    OTimerShow.Visibility = Visibility.Collapsed;
                    ProgressGrid.Visibility = Visibility.Collapsed;
                }
            }
            else if ((Application.Current as App).Timer_IsStart == 0)
            {
                TimerStartButtonIcon.Glyph = "\uF5B0";
                TimerEditButton.IsEnabled = true;

                if (TextHour.Text == "0" && TextMinute.Text == "0" && TextSecond.Text == "0")
                {
                    TimerStopButton.IsEnabled = false;
                }
                else
                {
                    TimerStopButton.IsEnabled = true;
                }

                OTimerShow.Visibility = Visibility.Collapsed;
                ProgressGrid.Visibility = Visibility.Collapsed;
            }

            //TimeDisplay
            if ((Application.Current as App).Timer_IsStart == 1)
            {
                long NowTicks = ((Application.Current as App).Timer_StartTick - DateTime.Now.Ticks / 10000);

                if (NowTicks <= 0)
                {
                    NowTicks = 0;
                    return;
                }

                int Hour = (int)(NowTicks / 1000 / 60 / 60);
                int Minute = (int)((NowTicks % (1000 * 60 * 60)) / 1000 / 60);
                int Second = (int)((NowTicks % (1000 * 60)) / 1000);

                TextHour.Text = Hour.ToString();
                TextMinute.Text = Minute.ToString();
                TextSecond.Text = Second.ToString();

                ProgressBar.Value = ((Application.Current as App).Timer_StartTick - DateTime.Now.Ticks / 10000) * 100.0 / (Application.Current as App).Timer_TotalTick;
            }
            else if ((Application.Current as App).Timer_IsStart == 2)
            {
                long NowTicks = ((Application.Current as App).Timer_PauseTick);

                if (NowTicks <= 0)
                {
                    NowTicks = 0;
                    return;
                }

                int Hour = (int)(NowTicks / 1000 / 60 / 60);
                int Minute = (int)((NowTicks % (1000 * 60 * 60)) / 1000 / 60);
                int Second = (int)((NowTicks % (1000 * 60)) / 1000);

                TextHour.Text = Hour.ToString();
                TextMinute.Text = Minute.ToString();
                TextSecond.Text = Second.ToString();

                ProgressBar.Value = ((Application.Current as App).Timer_PauseTick) * 100.0 / (Application.Current as App).Timer_TotalTick;
            }
            else if ((Application.Current as App).Timer_IsStart == 0)
            {
                TextHour.Text = (Application.Current as App).Timer_HourSet.ToString();
                TextMinute.Text = (Application.Current as App).Timer_MinuteSet.ToString();
                TextSecond.Text = (Application.Current as App).Timer_SecondSet.ToString();
            }

            TextO1.Text = (Application.Current as App).Timer_HourSet.ToString() + "时 " + (Application.Current as App).Timer_MinuteSet.ToString() + "分 " + (Application.Current as App).Timer_SecondSet.ToString() + "秒";
        }

        private void FileTimer()
        {
            (Application.Current as App).LocalSettings.Values["Timer_IsStart"] = (Application.Current as App).Timer_IsStart;
            (Application.Current as App).LocalSettings.Values["Timer_StartTick"] = (Application.Current as App).Timer_StartTick;
            (Application.Current as App).LocalSettings.Values["Timer_PauseTick"] = (Application.Current as App).Timer_PauseTick;
            (Application.Current as App).LocalSettings.Values["Timer_HourSet"] = (Application.Current as App).Timer_HourSet;
            (Application.Current as App).LocalSettings.Values["Timer_MinuteSet"] = (Application.Current as App).Timer_MinuteSet;
            (Application.Current as App).LocalSettings.Values["Timer_SecondSet"] = (Application.Current as App).Timer_SecondSet;
            (Application.Current as App).LocalSettings.Values["Timer_TotalTick"] = (Application.Current as App).Timer_TotalTick;
        }

        private void TimerStartButton_Click(object sender, RoutedEventArgs e)
        {
            if ((Application.Current as App).Timer_IsStart == 0 && !(TextHour.Text == "0" && TextMinute.Text == "0" && TextSecond.Text == "0"))
            {
                (Application.Current as App).Timer_IsStart = 1;
                (Application.Current as App).Timer_StartTick = (long)DateTime.Now.Ticks / 10000 + (Application.Current as App).Timer_HourSet * 60 * 60 * 1000 + (Application.Current as App).Timer_MinuteSet * 60 * 1000 + (Application.Current as App).Timer_SecondSet * 1000;
                (Application.Current as App).Timer_TotalTick = (Application.Current as App).Timer_HourSet * 60 * 60 * 1000 + (Application.Current as App).Timer_MinuteSet * 60 * 1000 + (Application.Current as App).Timer_SecondSet * 1000;
            }
            else if ((Application.Current as App).Timer_IsStart == 1)
            {
                (Application.Current as App).Timer_IsStart = 2;
                (Application.Current as App).Timer_PauseTick = (Application.Current as App).Timer_StartTick - (long)DateTime.Now.Ticks / 10000;
            }
            else if ((Application.Current as App).Timer_IsStart == 2)
            {
                (Application.Current as App).Timer_IsStart = 1;
                (Application.Current as App).Timer_StartTick = (long)DateTime.Now.Ticks / 10000 + (Application.Current as App).Timer_PauseTick;
                (Application.Current as App).Timer_PauseTick = 0;
            }
            FileTimer();
        }

        private async void TimerEditButton_Click(object sender, RoutedEventArgs e)
        {
            int NowH = (Application.Current as App).Timer_HourSet, NowM = (Application.Current as App).Timer_MinuteSet, NowS = (Application.Current as App).Timer_SecondSet;

            ContentDialog Dialog = new ContentDialog();
            Dialog.XamlRoot = this.XamlRoot;
            Dialog.Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style;
            Dialog.Title = "编辑计时器";
            Dialog.PrimaryButtonText = "保存";
            Dialog.CloseButtonText = "取消";
            Dialog.DefaultButton = ContentDialogButton.Primary;
            Dialog.Content = new EditTimerDialog();
            var result = await Dialog.ShowAsync();

            if (result != ContentDialogResult.None)
            {
                TextHour.Text = (Application.Current as App).Timer_HourSet.ToString();
                TextMinute.Text = (Application.Current as App).Timer_MinuteSet.ToString();
                TextSecond.Text = (Application.Current as App).Timer_SecondSet.ToString();

                if ((Application.Current as App).Timer_HourSet == 0 && (Application.Current as App).Timer_MinuteSet == 0 && (Application.Current as App).Timer_SecondSet == 0)
                {
                    TimerStopButton.IsEnabled = false;
                }
                else
                {
                    TimerStopButton.IsEnabled = true;
                }
            }
            else
            {
                (Application.Current as App).Timer_HourSet = NowH;
                (Application.Current as App).Timer_MinuteSet = NowM;
                (Application.Current as App).Timer_SecondSet = NowS;
            }
            FileTimer();
        }

        public void TimerStopButton_Click(object sender = null, RoutedEventArgs e = null)
        {
            OTimerShow.Visibility = Visibility.Collapsed;
            ProgressGrid.Visibility = Visibility.Collapsed;

            TextHour.Text = (Application.Current as App).Timer_HourSet.ToString();
            TextMinute.Text = (Application.Current as App).Timer_MinuteSet.ToString();
            TextSecond.Text = (Application.Current as App).Timer_SecondSet.ToString();
            if ((Application.Current as App).Timer_IsStart == 0)
            {
                TextHour.Text = "0";
                TextMinute.Text = "0";
                TextSecond.Text = "0";
                (Application.Current as App).Timer_HourSet = 0;
                (Application.Current as App).Timer_MinuteSet = 0;
                (Application.Current as App).Timer_SecondSet = 0;
            }
            else
            {
                (Application.Current as App).Timer_IsStart = 0;
                (Application.Current as App).Timer_StartTick = 0;
                (Application.Current as App).Timer_PauseTick = 0;
            }
            FileTimer();
        }

        private void Page_SizeChanged(object sender = null, SizeChangedEventArgs e = null)
        {
            if (ActualHeight <= 240 || ActualWidth <= 400)
            {
                TimerControls.Visibility = Visibility.Collapsed;

                if (ActualWidth > 0 && ActualHeight > 0)
                    TimeDisplay.FontSize = (ActualWidth / 7 > ActualHeight / 3) ? ActualHeight / 3 : ActualWidth / 7;
                TextHour.FontSize = TimeDisplay.FontSize;
                TextMinute.FontSize = TimeDisplay.FontSize;
                TextSecond.FontSize = TimeDisplay.FontSize;
                GridHour.Height = TimeDisplay.FontSize * 1.2;
                GridMinute.Height = TimeDisplay.FontSize * 1.2;
                GridSecond.Height = TimeDisplay.FontSize * 1.2;

                TextH1.FontSize = TextH2.FontSize = TextH3.FontSize = 10;
                TextH1.Margin = new Thickness(4, 0, 6, TimeDisplay.FontSize / 2.0 - 08);
                TextH2.Margin = new Thickness(4, 0, 6, TimeDisplay.FontSize / 2.0 - 08);
                TextH3.Margin = new Thickness(4, 0, 0, TimeDisplay.FontSize / 2.0 - 08);
                GridTime.Margin = new Thickness(0, 16 + TimeDisplay.FontSize / 10.0, 0, -16);
                return;
            }
            else
            {
                TimerControls.Visibility = Visibility.Visible;
            }

            if (ApplicationView.GetForCurrentView().ViewMode == ApplicationViewMode.CompactOverlay)
            {
                TimerEditButton.Visibility = Visibility.Collapsed;
            }
            else
            {
                TimerEditButton.Visibility = Visibility.Visible;
            }

            if (ActualWidth > 0 && ActualHeight - 60 - 32 > 0)
            {
                if ((ActualHeight - 60 - 32) / 1.5 > (ActualWidth) / 7)
                {
                    TimeDisplay.FontSize = (ActualWidth) * 1.0 / 7;
                }
                else
                {
                    TimeDisplay.FontSize = (ActualHeight - 60 - 32) / 1.5;
                }
                if (!(TimeDisplay.FontSize > 0))
                {
                    TimeDisplay.FontSize = 1;
                }
            }
            if (ActualWidth <= 600 || (ActualHeight - 60 - 32) <= 100)
            {
                TextH1.FontSize = TextH2.FontSize = TextH3.FontSize = 20;
                TextH1.Margin = new Thickness(10, 0, 20, TimeDisplay.FontSize / 2.0 - 10);
                TextH2.Margin = new Thickness(10, 0, 20, TimeDisplay.FontSize / 2.0 - 10);
                TextH3.Margin = new Thickness(10, 0, 0, TimeDisplay.FontSize / 2.0 - 10);
            }
            else
            {
                TextH1.FontSize = TextH2.FontSize = TextH3.FontSize = 50;
                TextH1.Margin = new Thickness(10, 0, 20, TimeDisplay.FontSize / 3.0 - 10);
                TextH2.Margin = new Thickness(10, 0, 20, TimeDisplay.FontSize / 3.0 - 10);
                TextH3.Margin = new Thickness(10, 0, 0, TimeDisplay.FontSize / 3.0 - 10);
            }
            TextHour.FontSize = TimeDisplay.FontSize;
            TextMinute.FontSize = TimeDisplay.FontSize;
            TextSecond.FontSize = TimeDisplay.FontSize;
            GridHour.Height = TimeDisplay.FontSize * 1.2;
            GridMinute.Height = TimeDisplay.FontSize * 1.2;
            GridSecond.Height = TimeDisplay.FontSize * 1.2;
            GridTime.Margin = new Thickness(0, 32 + TimeDisplay.FontSize / 10.0, 0, 60);
        }
    }
}
