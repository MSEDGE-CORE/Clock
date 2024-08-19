using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
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
    public sealed partial class StopWatch : Page
    {
        DispatcherTimer Timer1;

        public StopWatch()
        {
            this.InitializeComponent();

            TextHour.Text = "0";
            TextMinute.Text = "00";
            TextSecond.Text = "00";
            TextMSecond.Text = "00";

            ListFlag.ItemsSource = (Application.Current as App).SwFlagList;

            Timer1 = new DispatcherTimer();
            Timer1.Interval = new TimeSpan(0, 0, 0, 0, 2);
            Timer1.Tick += Timer_Tick;
            Timer1.Start();
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
                StopWatchFlagButton.IsEnabled = false;
            }

            //TimeDisplay
            if ((Application.Current as App).StopWatch_IsStart == 1)
            {
                long NowTicks = (DateTime.Now.Ticks / 10000 - (Application.Current as App).StopWatch_StartTick);

                if (NowTicks <= 0)
                {
                    NowTicks = 0;
                    return;
                }

                int Hour = (int)(NowTicks / 1000 / 60 / 60);
                int Minute = (int)((NowTicks % (1000 * 60 * 60)) / 1000 / 60);
                int Second = (int)((NowTicks % (1000 * 60)) / 1000);
                int MSecond = (int)((NowTicks % (1000)) / 10);

                string sHour = Hour.ToString();
                string s0 = "";
                if (Minute / 10 == 0)
                    s0 = "0";
                else
                    s0 = "";
                string sMinute = s0 + Minute.ToString();
                if (Second / 10 == 0)
                    s0 = "0";
                else
                    s0 = "";
                string sSecond = s0 + Second.ToString();
                if (MSecond / 10 == 0)
                    s0 = "0";
                else
                    s0 = "";
                string sMSecond = s0 + MSecond.ToString();

                TextHour.Text = sHour;
                TextMinute.Text = sMinute;
                TextSecond.Text = sSecond;
                TextMSecond.Text = sMSecond;

                if (Hour == 0)
                {
                    TextHour.Visibility = Visibility.Collapsed;
                    TextH1.Visibility = Visibility.Collapsed;
                }
                else
                {
                    TextHour.Visibility = Visibility.Visible;
                    TextH1.Visibility = Visibility.Visible;
                }
            }
            else if ((Application.Current as App).StopWatch_IsStart == 2)
            {
                long NowTicks = ((Application.Current as App).StopWatch_PauseTick);

                if (NowTicks <= 0)
                {
                    NowTicks = 0;
                    return;
                }

                int Hour = (int)(NowTicks / 1000 / 60 / 60);
                int Minute = (int)((NowTicks % (1000 * 60 * 60)) / 1000 / 60);
                int Second = (int)((NowTicks % (1000 * 60)) / 1000);
                int MSecond = (int)((NowTicks % (1000)) / 10);

                string sHour = Hour.ToString();
                string s0 = "";
                if (Minute / 10 == 0)
                    s0 = "0";
                else
                    s0 = "";
                string sMinute = s0 + Minute.ToString();
                if (Second / 10 == 0)
                    s0 = "0";
                else
                    s0 = "";
                string sSecond = s0 + Second.ToString();
                if (MSecond / 10 == 0)
                    s0 = "0";
                else
                    s0 = "";
                string sMSecond = s0 + MSecond.ToString();

                TextHour.Text = sHour;
                TextMinute.Text = sMinute;
                TextSecond.Text = sSecond;
                TextMSecond.Text = sMSecond;

                if (Hour == 0)
                {
                    TextHour.Visibility = Visibility.Collapsed;
                    TextH1.Visibility = Visibility.Collapsed;
                }
                else
                {
                    TextHour.Visibility = Visibility.Visible;
                    TextH1.Visibility = Visibility.Visible;
                }
            }
            else if ((Application.Current as App).StopWatch_IsStart == 0)
            {
                TextHour.Text = "0";
                TextMinute.Text = "00";
                TextSecond.Text = "00";
                TextMSecond.Text = "00";
                TextHour.Visibility = Visibility.Collapsed;
                TextH1.Visibility = Visibility.Collapsed;
            }
        }

        private void FileStopWatch()
        {
            (Application.Current as App).LocalSettings.Values["StopWatch_IsStart"] = (Application.Current as App).StopWatch_IsStart;
            (Application.Current as App).LocalSettings.Values["StopWatch_StartTick"] = (Application.Current as App).StopWatch_StartTick;
            (Application.Current as App).LocalSettings.Values["StopWatch_PauseTick"] = (Application.Current as App).StopWatch_PauseTick;
            (Application.Current as App).LocalSettings.Values["StopWatch_LastFlagTick"] = (Application.Current as App).StopWatch_LastFlagTick;
        }

        private void StopWatchStartButton_Click(object sender, RoutedEventArgs e)
        {
            if ((Application.Current as App).StopWatch_IsStart == 0)
            {
                (Application.Current as App).StopWatch_IsStart = 1;
                (Application.Current as App).StopWatch_StartTick = (long)DateTime.Now.Ticks / 10000;
            }
            else if ((Application.Current as App).StopWatch_IsStart == 1)
            {
                (Application.Current as App).StopWatch_IsStart = 2;
                (Application.Current as App).StopWatch_PauseTick = (long)DateTime.Now.Ticks / 10000 - (Application.Current as App).StopWatch_StartTick;
            }
            else if ((Application.Current as App).StopWatch_IsStart == 2)
            {
                (Application.Current as App).StopWatch_IsStart = 1;
                (Application.Current as App).StopWatch_StartTick = (long)DateTime.Now.Ticks / 10000 - (Application.Current as App).StopWatch_PauseTick;
                (Application.Current as App).StopWatch_PauseTick = 0;
            }

            Page_SizeChanged();
            FileStopWatch();
        }

        private async void StopWatchStopButton_Click(object sender, RoutedEventArgs e)
        {
            TextHour.Text = "0";
            TextMinute.Text = "00";
            TextSecond.Text = "00";
            TextMSecond.Text = "00";

            (Application.Current as App).StopWatch_IsStart = 0;
            (Application.Current as App).StopWatch_StartTick = 0;
            (Application.Current as App).StopWatch_PauseTick = 0;
            (Application.Current as App).StopWatch_LastFlagTick = 0;

            (Application.Current as App).SwFlagList.Clear();

            Page_SizeChanged();
            FileStopWatch();

            try
            {
                Windows.Storage.StorageFolder StorageFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
                StorageFolder = await StorageFolder.GetFolderAsync("StopWatchFlags");
                await StorageFolder.DeleteAsync();
            }
            catch { }
        }

        int Minute = 0, Second = 0, MSecond = 0;
        string s0 = "", sMinute = "00", sSecond = "00", sMSecond = "00";
        private void GetHMS(long NowTicks = 0)
        {
            if (NowTicks <= 0)
            {
                NowTicks = 0;
                return;
            }
            Minute = (int)((NowTicks) / 1000 / 60);
            Second = (int)((NowTicks % (1000 * 60)) / 1000);
            MSecond = (int)((NowTicks % (1000)) / 10);
            if (Minute / 10 == 0)
                s0 = "0";
            else
                s0 = "";
            sMinute = s0 + Minute.ToString();
            if (Second / 10 == 0)
                s0 = "0";
            else
                s0 = "";
            sSecond = s0 + Second.ToString();
            if (MSecond / 10 == 0)
                s0 = "0";
            else
                s0 = "";
            sMSecond = s0 + MSecond.ToString();
        }

        private async void StopWatchFlagButton_Click(object sender, RoutedEventArgs e)
        {
            long NowTicks = (DateTime.Now.Ticks / 10000 - (Application.Current as App).StopWatch_StartTick);
            GetHMS(NowTicks);
            string Total = sMinute + ":" + sSecond + "." + sMSecond; 

            long NowTicksTmp = NowTicks;
            NowTicks = (NowTicksTmp - (Application.Current as App).StopWatch_LastFlagTick);
            (Application.Current as App).StopWatch_LastFlagTick = NowTicksTmp;
            GetHMS(NowTicks);
            string Plus = sMinute + ":" + sSecond + "." + sMSecond;

            (Application.Current as App).SwFlagList.Insert(0, new SwFlag_List { Num = ((Application.Current as App).SwFlagList.Count() + 1).ToString(), Total = Total, Plus = Plus });

            Page_SizeChanged();
            FileStopWatch();

            try
            {
                Windows.Storage.StorageFolder StorageFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
                Windows.Storage.StorageFile FileCount = await StorageFolder.CreateFileAsync("StopWatchFlags\\Count.txt", Windows.Storage.CreationCollisionOption.OpenIfExists);
                Windows.Storage.StorageFile FileFlags = await StorageFolder.CreateFileAsync("StopWatchFlags\\Flag" + (Application.Current as App).SwFlagList[0].Num + ".txt", Windows.Storage.CreationCollisionOption.OpenIfExists);
                await Windows.Storage.FileIO.WriteTextAsync(FileCount, (Application.Current as App).SwFlagList.Count.ToString());
                await Windows.Storage.FileIO.WriteTextAsync(FileFlags, (Application.Current as App).SwFlagList[0].Total + "\n" + (Application.Current as App).SwFlagList[0].Plus + "\n");
            }
            catch { }
        }

        int RightMargin = 0;
        private void SetFontSize()
        {
            if (ActualWidth - RightMargin > 0 && ActualHeight - 60 - 32 > 0)
            {
                if ((ActualHeight - 60 - 32) / 1.5 > (ActualWidth - RightMargin) / 7)
                {
                    TimeDisplay.FontSize = (ActualWidth - RightMargin) * 1.0 / 7;
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
            if (ActualWidth - RightMargin <= 600 || (ActualHeight - 60 - 32) <= 100)
            {
                TextH1.FontSize = TextH2.FontSize = TextH3.FontSize = 20;
                TextH1.Margin = new Thickness(5, 0, 10, TimeDisplay.FontSize / 2.0 - 10);
                TextH2.Margin = new Thickness(5, 0, 10, TimeDisplay.FontSize / 2.0 - 10);
                TextH3.Margin = new Thickness(5, 0, 10, TimeDisplay.FontSize / 2.0 - 10);
            }
            else
            {
                TextH1.FontSize = TextH2.FontSize = TextH3.FontSize = 50;
                TextH1.Margin = new Thickness(5, 0, 10, TimeDisplay.FontSize / 3.0 - 10);
                TextH2.Margin = new Thickness(5, 0, 10, TimeDisplay.FontSize / 3.0 - 10);
                TextH3.Margin = new Thickness(5, 0, 10, TimeDisplay.FontSize / 3.0 - 10);
            }
            TextHour.FontSize = TimeDisplay.FontSize;
            TextMinute.FontSize = TimeDisplay.FontSize;
            TextSecond.FontSize = TimeDisplay.FontSize;
            TextMSecond.FontSize = TimeDisplay.FontSize;
            GridHour.Height = TimeDisplay.FontSize * 1.2;
            GridMinute.Height = TimeDisplay.FontSize * 1.2;
            GridSecond.Height = TimeDisplay.FontSize * 1.2;
            GridMSecond.Height = TimeDisplay.FontSize * 1.2;
        }

        private void Page_SizeChanged(object sender = null, SizeChangedEventArgs e = null)
        {
            if((Application.Current as App).SwFlagList.Count() == 0)
            {
                RightMargin = 0;
                GridTime.VerticalAlignment = VerticalAlignment.Center;
                ListFlag.Visibility = Visibility.Visible;
                FlagSeparator.Visibility = Visibility.Collapsed;
                if (ActualWidth <= 680)
                {
                    ListFlag.Width = 300;
                    ListFlag.Margin = new Thickness(0, TimeDisplay.FontSize * 1.2 + ActualHeight / 2, 0, 0);
                    ListFlag.HorizontalAlignment = HorizontalAlignment.Stretch;
                }
                else
                {
                    ListFlag.Width = 280;
                    ListFlag.Margin = new Thickness(0, 32, 0, 0);
                    ListFlag.HorizontalAlignment = HorizontalAlignment.Right;
                }
                SetFontSize();
                GridTime.Margin = new Thickness(0, 48, 0, 12);
            }
            else if (ActualHeight - 60 - 32 <= 200 && ActualWidth <= 680)
            {
                RightMargin = 0;
                ListFlag.Visibility = Visibility.Visible;
                FlagSeparator.Visibility = Visibility.Collapsed;
                GridTime.VerticalAlignment = VerticalAlignment.Center;
                ListFlag.Margin = new Thickness(0, ActualHeight, 0, 60);
                ListFlag.HorizontalAlignment = HorizontalAlignment.Stretch;
                ListFlag.VerticalAlignment = VerticalAlignment.Stretch;
                ListFlag.Width = 300;
                SetFontSize();
                GridTime.Margin = new Thickness(0, 32 + TimeDisplay.FontSize / 10.0, 0, 60);
            }
            else if(ActualWidth <= 680)
            {
                RightMargin = 0;
                SetFontSize();
                ListFlag.Visibility = Visibility.Visible;
                FlagSeparator.Visibility = Visibility.Collapsed;
                GridTime.VerticalAlignment = VerticalAlignment.Top;
                ListFlag.Margin = new Thickness(0, TextMinute.FontSize + 60, 0, 60);
                ListFlag.HorizontalAlignment = HorizontalAlignment.Stretch;
                ListFlag.VerticalAlignment = VerticalAlignment.Stretch;
                ListFlag.Width = 300;
                GridTime.Margin = new Thickness(0, 48, 0, 12);
            }
            else
            {
                RightMargin = 280;
                ListFlag.Visibility = Visibility.Visible;
                FlagSeparator.Visibility = Visibility.Visible;
                GridTime.VerticalAlignment = VerticalAlignment.Center;
                ListFlag.Margin = new Thickness(0, 32, 0, 0);
                if ((ActualWidth - 280) < (ActualWidth / 2 + 48 + 12 + 24))
                    ListFlag.Margin = new Thickness(0, 32, 0, 60);
                ListFlag.Width = 280;
                ListFlag.HorizontalAlignment = HorizontalAlignment.Right;
                ListFlag.VerticalAlignment = VerticalAlignment.Stretch;
                SetFontSize();
                GridTime.Margin = new Thickness(0, 32 + TimeDisplay.FontSize / 10.0, 280, 60);
            }
        }
    }
}
