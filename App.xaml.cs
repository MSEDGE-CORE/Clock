using Microsoft.Toolkit.Uwp.Notifications;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Devices.PointOfService;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI;
using Windows.UI.Notifications;
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
    public class SwFlag_List
    {
        public string Num { get; set; }
        public string Total { get; set; }
        public string Plus { get; set; }
    }

    sealed partial class App : Application
    {
        public Frame RootFrame;
        public ApplicationDataContainer LocalSettings = Windows.Storage.ApplicationData.Current.LocalSettings;

        public int StopWatch_IsStart = 0; 
        public long StopWatch_StartTick = 0;
        public long StopWatch_PauseTick = 0;
        public long StopWatch_LastFlagTick = 0;

        public int Timer_IsStart = 0; 
        public long Timer_StartTick = 0;
        public long Timer_PauseTick = 0;
        public int Timer_HourSet = 0;
        public int Timer_MinuteSet = 0;
        public int Timer_SecondSet = 0;
        public long Timer_TotalTick = 0;

        public ObservableCollection<SwFlag_List> SwFlagList { get; } = new ObservableCollection<SwFlag_List>();

        public App()
        {
            this.InitializeComponent();
            this.Suspending += OnSuspending;
        }

        protected override void OnLaunched(LaunchActivatedEventArgs e = null)
        {
            RootFrame = Window.Current.Content as Frame;

            if (RootFrame == null)
            {
                RootFrame = new Frame();
                RootFrame.NavigationFailed += OnNavigationFailed;
                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {

                }
                Window.Current.Content = RootFrame;
            }

            if (e.PrelaunchActivated == false)
            {
                if (RootFrame.Content == null)
                {
                    RootFrame.Navigate(typeof(NavPage), e.Arguments);
                }
                Window.Current.Activate();
            }

            GetSettings();
        }

        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            deferral.Complete();
        }

        public async void GetSettings()
        {
            try
            {
                ApplicationViewTitleBar TitleBar = ApplicationView.GetForCurrentView().TitleBar;
                if (LocalSettings.Values["Theme"] == null || (int)LocalSettings.Values["Theme"] == 0)
                {
                    (Application.Current as App).RootFrame.RequestedTheme = ElementTheme.Default;
                    TitleBar.ButtonForegroundColor = default;
                    TitleBar.ButtonHoverForegroundColor = default;
                    TitleBar.ButtonPressedForegroundColor = default;
                    TitleBar.ButtonInactiveForegroundColor = default;
                    TitleBar.ButtonHoverBackgroundColor = default;
                    TitleBar.ButtonPressedBackgroundColor = default;
                    LocalSettings.Values["Theme"] = 0;
                }
                else if ((int)LocalSettings.Values["Theme"] == 1)
                {
                    (Application.Current as App).RootFrame.RequestedTheme = ElementTheme.Light;
                    TitleBar.ButtonForegroundColor = Colors.Black;
                    TitleBar.ButtonHoverForegroundColor = Colors.Black;
                    TitleBar.ButtonPressedForegroundColor = Colors.Black;
                    TitleBar.ButtonInactiveForegroundColor = Colors.Gray;
                    TitleBar.ButtonHoverBackgroundColor = Colors.White;
                    TitleBar.ButtonPressedBackgroundColor = Colors.LightGray;
                }
                else if ((int)LocalSettings.Values["Theme"] == 2)
                {
                    (Application.Current as App).RootFrame.RequestedTheme = ElementTheme.Dark;
                    TitleBar.ButtonForegroundColor = Colors.White;
                    TitleBar.ButtonHoverForegroundColor = Colors.White;
                    TitleBar.ButtonPressedForegroundColor = Colors.White;
                    TitleBar.ButtonInactiveForegroundColor = Colors.DarkGray;
                    TitleBar.ButtonHoverBackgroundColor = Colors.Black;
                    TitleBar.ButtonPressedBackgroundColor = Colors.Gray;
                }

                if (LocalSettings.Values["Timer_IsStart"] != null)
                {
                    (Application.Current as App).Timer_IsStart = (int)(Application.Current as App).LocalSettings.Values["Timer_IsStart"];
                    (Application.Current as App).Timer_StartTick = (long)(Application.Current as App).LocalSettings.Values["Timer_StartTick"];
                    (Application.Current as App).Timer_PauseTick = (long)(Application.Current as App).LocalSettings.Values["Timer_PauseTick"];
                    (Application.Current as App).Timer_HourSet = (int)(Application.Current as App).LocalSettings.Values["Timer_HourSet"];
                    (Application.Current as App).Timer_MinuteSet = (int)(Application.Current as App).LocalSettings.Values["Timer_MinuteSet"];
                    (Application.Current as App).Timer_SecondSet = (int)(Application.Current as App).LocalSettings.Values["Timer_SecondSet"];
                    (Application.Current as App).Timer_TotalTick = (long)(Application.Current as App).LocalSettings.Values["Timer_TotalTick"];
                }
                else
                {
                    (Application.Current as App).LocalSettings.Values["Timer_IsStart"] = (Application.Current as App).Timer_IsStart;
                    (Application.Current as App).LocalSettings.Values["Timer_StartTick"] = (Application.Current as App).Timer_StartTick;
                    (Application.Current as App).LocalSettings.Values["Timer_PauseTick"] = (Application.Current as App).Timer_PauseTick;
                    (Application.Current as App).LocalSettings.Values["Timer_HourSet"] = (Application.Current as App).Timer_HourSet;
                    (Application.Current as App).LocalSettings.Values["Timer_MinuteSet"] = (Application.Current as App).Timer_MinuteSet;
                    (Application.Current as App).LocalSettings.Values["Timer_SecondSet"] = (Application.Current as App).Timer_SecondSet;
                    (Application.Current as App).LocalSettings.Values["Timer_TotalTick"] = (Application.Current as App).Timer_TotalTick;
                }

                if (LocalSettings.Values["StopWatch_IsStart"] != null)
                {
                    (Application.Current as App).StopWatch_IsStart = (int)(Application.Current as App).LocalSettings.Values["StopWatch_IsStart"];
                    (Application.Current as App).StopWatch_StartTick = (long)(Application.Current as App).LocalSettings.Values["StopWatch_StartTick"];
                    (Application.Current as App).StopWatch_PauseTick = (long)(Application.Current as App).LocalSettings.Values["StopWatch_PauseTick"];
                    (Application.Current as App).StopWatch_LastFlagTick = (long)(Application.Current as App).LocalSettings.Values["StopWatch_LastFlagTick"];
                }
                else
                {
                    (Application.Current as App).LocalSettings.Values["StopWatch_IsStart"] = (Application.Current as App).StopWatch_IsStart;
                    (Application.Current as App).LocalSettings.Values["StopWatch_StartTick"] = (Application.Current as App).StopWatch_StartTick;
                    (Application.Current as App).LocalSettings.Values["StopWatch_PauseTick"] = (Application.Current as App).StopWatch_PauseTick;
                    (Application.Current as App).LocalSettings.Values["StopWatch_LastFlagTick"] = (Application.Current as App).StopWatch_LastFlagTick;
                }

                Windows.Storage.StorageFolder StorageFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
                int StopWatchCount = 0;
                Windows.Storage.StorageFile file;
                try
                {
                    file = await StorageFolder.GetFileAsync("StopWatchFlags\\Count.txt");
                    var Count = await Windows.Storage.FileIO.ReadLinesAsync(file);
                    StopWatchCount = Int32.Parse(Count[0]);
                }
                catch { }
                if (StopWatchCount > 0)
                {
                    for (int i = 1; i <= StopWatchCount; i++)
                    {
                        string FileCollectionTitle = "StopWatchFlags\\Flag" + i.ToString() + ".txt";
                        StorageFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
                        try
                        {
                            file = await StorageFolder.GetFileAsync(FileCollectionTitle);
                            var FileFlagsList = await Windows.Storage.FileIO.ReadLinesAsync(file);
                            this.SwFlagList.Insert(0, new SwFlag_List() { Num = i.ToString(), Total = FileFlagsList[0], Plus = FileFlagsList[1] });
                        }
                        catch { }
                    }
                }
            }
            catch { }
        }
    }
}
