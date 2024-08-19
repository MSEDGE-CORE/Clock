using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Microsoft.Toolkit.Uwp.Notifications;
using Windows.UI.ViewManagement;
using Windows.ApplicationModel.Core;
using Windows.UI;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace Clock
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class Timer : Page
    {
        DispatcherTimer Timer1;

        int ThemeSelected = 0;

        public Timer()
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

            Timer1 = new DispatcherTimer();
            Timer1.Interval = new TimeSpan(0, 0, 0, 0, 5);
            Timer1.Tick += Timer_Tick;
            Timer1.Start();
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

        private void Timer_Start(object sender, RoutedEventArgs e)
        {
            if ((Application.Current as App).Timer_IsStart == 0 && ((Application.Current as App).Timer_HourSet != 0 || (Application.Current as App).Timer_MinuteSet != 0 || (Application.Current as App).Timer_SecondSet != 0))
            {
                TimerStartButtonIcon.Glyph = "\uF8AE";
                TimerStopButton.IsEnabled = true;
                TimerEditButton.IsEnabled = false;
                (Application.Current as App).Timer_IsStart = 1;
                (Application.Current as App).Timer_StartTick = (long)DateTime.Now.Ticks / 10000 + (Application.Current as App).Timer_HourSet * 60 * 60 * 1000 + (Application.Current as App).Timer_MinuteSet * 60 * 1000 + (Application.Current as App).Timer_SecondSet * 1000;

                ApplicationDataContainer LocalSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
                Windows.Storage.ApplicationDataCompositeValue Timer_Status = new Windows.Storage.ApplicationDataCompositeValue();
                Timer_Status["IsStart"] = (Application.Current as App).Timer_IsStart;
                Timer_Status["StartTick"] = (Application.Current as App).Timer_StartTick;
                Timer_Status["PauseTick"] = (Application.Current as App).Timer_PauseTick;
                LocalSettings.Values["Timer_Status"] = Timer_Status;
            }
            else if ((Application.Current as App).Timer_IsStart == 1)
            {
                TimerStartButtonIcon.Glyph = "\uF5B0";
                TimerStopButton.IsEnabled = true;
                TimerEditButton.IsEnabled = false;
                (Application.Current as App).Timer_IsStart = 2;
                (Application.Current as App).Timer_PauseTick = (Application.Current as App).Timer_StartTick - (long)DateTime.Now.Ticks / 10000;

                ApplicationDataContainer LocalSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
                Windows.Storage.ApplicationDataCompositeValue Timer_Status = new Windows.Storage.ApplicationDataCompositeValue();
                Timer_Status["IsStart"] = (Application.Current as App).Timer_IsStart;
                Timer_Status["StartTick"] = (Application.Current as App).Timer_StartTick;
                Timer_Status["PauseTick"] = (Application.Current as App).Timer_PauseTick;
                LocalSettings.Values["Timer_Status"] = Timer_Status;
            }
            else if ((Application.Current as App).Timer_IsStart == 2)
            {
                TimerStartButtonIcon.Glyph = "\uF8AE";
                TimerStopButton.IsEnabled = true;
                TimerEditButton.IsEnabled = false;
                (Application.Current as App).Timer_IsStart = 1;
                (Application.Current as App).Timer_StartTick = (long)DateTime.Now.Ticks / 10000 + (Application.Current as App).Timer_PauseTick;
                (Application.Current as App).Timer_PauseTick = 0;

                ApplicationDataContainer LocalSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
                Windows.Storage.ApplicationDataCompositeValue Timer_Status = new Windows.Storage.ApplicationDataCompositeValue();
                Timer_Status["IsStart"] = (Application.Current as App).Timer_IsStart;
                Timer_Status["StartTick"] = (Application.Current as App).Timer_StartTick;
                Timer_Status["PauseTick"] = (Application.Current as App).Timer_PauseTick;
                LocalSettings.Values["Timer_Status"] = Timer_Status;
            }
        }

        private async void Timer_Edit(object sender, RoutedEventArgs e)
        {
            ContentDialog Dialog = new ContentDialog();
            Dialog.Title = "编辑计时器";
            Dialog.CloseButtonText = "保存";
            Dialog.DefaultButton = ContentDialogButton.Close;
            Dialog.Content = new EditTimerDialog();

            var result = await Dialog.ShowAsync();

            ApplicationDataContainer LocalSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            Windows.Storage.ApplicationDataCompositeValue Timer_Set = (ApplicationDataCompositeValue)LocalSettings.Values["Timer_Set"];
            if (Timer_Set != null)
            {
                (Application.Current as App).Timer_HourSet = (int)Timer_Set["Hour"];
                (Application.Current as App).Timer_MinuteSet = (int)Timer_Set["Minute"];
                (Application.Current as App).Timer_SecondSet = (int)Timer_Set["Second"];
            }
            if ((Application.Current as App).Timer_HourSet == 0 && (Application.Current as App).Timer_MinuteSet == 0 && (Application.Current as App).Timer_SecondSet == 0)
            {
                TimerStopButton.IsEnabled = false;
            }
            else
            {
                TimerStopButton.IsEnabled = true;
            }
        }

        private void Timer_Stop(object sender, RoutedEventArgs e)
        {
            if((Application.Current as App).Timer_IsStart == 0)
            {
                TimerStopButton.IsEnabled = false;

                (Application.Current as App).Timer_HourSet = 0;
                (Application.Current as App).Timer_MinuteSet = 0;
                (Application.Current as App).Timer_SecondSet = 0;
                ApplicationDataContainer LocalSettings1 = Windows.Storage.ApplicationData.Current.LocalSettings;
                Windows.Storage.ApplicationDataCompositeValue Timer_Set = new Windows.Storage.ApplicationDataCompositeValue();
                Timer_Set["Hour"] = 0;
                Timer_Set["Minute"] = 0;
                Timer_Set["Second"] = 0;
                LocalSettings1.Values["Timer_Set"] = Timer_Set;
            }
            else
            {
                TimerStartButtonIcon.Glyph = "\uF5B0";
                TimerStopButton.IsEnabled = true;
                TimerEditButton.IsEnabled = true;
                (Application.Current as App).Timer_IsStart = 0;
                (Application.Current as App).Timer_StartTick = 0;
                (Application.Current as App).Timer_PauseTick = 0;
                TimeDisplay.Text = "00:00:00";

                ApplicationDataContainer LocalSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
                Windows.Storage.ApplicationDataCompositeValue Timer_Status = new Windows.Storage.ApplicationDataCompositeValue();
                Timer_Status["IsStart"] = (Application.Current as App).Timer_IsStart;
                Timer_Status["StartTick"] = (Application.Current as App).Timer_StartTick;
                Timer_Status["PauseTick"] = (Application.Current as App).Timer_PauseTick;
                LocalSettings.Values["Timer_Status"] = Timer_Status;
            }
        }

        private void Timer_Tick(object sender, object e)
        {
            if ((Application.Current as App).Timer_IsStart == 1)
            {
                TimerStartButtonIcon.Glyph = "\uF8AE";
                TimerStopButton.IsEnabled = true;
                TimerEditButton.IsEnabled = false;
            }
            else if ((Application.Current as App).Timer_IsStart == 2)
            {
                TimerStartButtonIcon.Glyph = "\uF5B0";
                TimerStopButton.IsEnabled = true;
                TimerEditButton.IsEnabled = false;
            }
            else if ((Application.Current as App).Timer_IsStart == 0)
            {
                TimerStartButtonIcon.Glyph = "\uF5B0";
                TimerEditButton.IsEnabled = true;

                if ((Application.Current as App).Timer_HourSet == 0 && (Application.Current as App).Timer_MinuteSet == 0 && (Application.Current as App).Timer_SecondSet == 0)
                {
                    TimerStopButton.IsEnabled = false;
                }
                else
                {
                    TimerStopButton.IsEnabled = true;
                }
            }

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

                if ((ActualHeight - 64 - 24) / 1.5 > ActualWidth / 6)
                {
                    TimeDisplay.FontSize = ActualWidth * 1.0 / 6;
                }
                else
                {
                    TimeDisplay.FontSize = (ActualHeight - 64 - 24) / 1.5;
                }
                if (!(TimeDisplay.FontSize > 0))
                {
                    TimeDisplay.FontSize = 1;
                }

                SeparateLineLight.Y1 = ActualHeight - 64 - 24;
                SeparateLineLight.Y2 = ActualHeight - 64 - 24;
                SeparateLineDark.Y1 = ActualHeight - 64 - 24;
                SeparateLineDark.Y2 = ActualHeight - 64 - 24;
            }

            if((Application.Current as App).Timer_IsStart == 1)
            {
                long NowTicks = ((Application.Current as App).Timer_StartTick - DateTime.Now.Ticks / 10000);

                if(NowTicks <= 0)
                {
                    NowTicks = 0;
                }

                int Hour = (int)(NowTicks / 1000 / 60 / 60);
                int Minute = (int)((NowTicks % (1000 * 60 * 60)) / 1000 / 60);
                int Second = (int)((NowTicks % (1000 * 60)) / 1000);

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

                TimeDisplay.Text = DisplayText.ToString();
            }
            else if((Application.Current as App).Timer_IsStart == 2)
            {
                long NowTicks =  ((Application.Current as App).Timer_PauseTick);

                int Hour = (int)(NowTicks / 1000 / 60 / 60);
                int Minute = (int)((NowTicks % (1000 * 60 * 60)) / 1000 / 60);
                int Second = (int)((NowTicks % (1000 * 60)) / 1000);

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

                TimeDisplay.Text = DisplayText.ToString();
            }
            else if((Application.Current as App).Timer_IsStart == 0) 
            {
                string DisplayText = "";

                if ((Application.Current as App).Timer_HourSet >= 10 && (Application.Current as App).Timer_HourSet <= 99)
                {
                    DisplayText += ((Application.Current as App).Timer_HourSet / 10).ToString();
                    DisplayText += ((Application.Current as App).Timer_HourSet % 10).ToString();
                }
                else if ((Application.Current as App).Timer_HourSet >= 0 && (Application.Current as App).Timer_HourSet <= 9)
                {
                    DisplayText += '0';
                    DisplayText += ((Application.Current as App).Timer_HourSet % 10).ToString();
                }
                DisplayText += ':';
                if ((Application.Current as App).Timer_MinuteSet >= 10 && (Application.Current as App).Timer_MinuteSet <= 59)
                {
                    DisplayText += ((Application.Current as App).Timer_MinuteSet / 10).ToString();
                    DisplayText += ((Application.Current as App).Timer_MinuteSet % 10).ToString();
                }
                else if ((Application.Current as App).Timer_MinuteSet >= 0 && (Application.Current as App).Timer_MinuteSet <= 9)
                {
                    DisplayText += '0';
                    DisplayText += ((Application.Current as App).Timer_MinuteSet % 10).ToString();
                }
                DisplayText += ':';
                if ((Application.Current as App).Timer_SecondSet >= 10 && (Application.Current as App).Timer_SecondSet <= 59)
                {
                    DisplayText += ((Application.Current as App).Timer_SecondSet / 10).ToString();
                    DisplayText += ((Application.Current as App).Timer_SecondSet % 10).ToString();
                }
                else if ((Application.Current as App).Timer_SecondSet >= 0 && (Application.Current as App).Timer_SecondSet <= 9)
                {
                    DisplayText += '0';
                    DisplayText += ((Application.Current as App).Timer_SecondSet % 10).ToString();
                }

                TimeDisplay.Text = DisplayText;
            }
        }
    }
}
