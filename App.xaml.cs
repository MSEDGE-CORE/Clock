using Microsoft.Toolkit.Uwp.Notifications;
using System;
using System.Collections.Generic;
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
    /// <summary>
    /// 提供特定于应用程序的行为，以补充默认的应用程序类。
    /// </summary>
    sealed partial class App : Application
    {
        /// <summary>
        /// 初始化单一实例应用程序对象。这是执行的创作代码的第一行，
        /// 已执行，逻辑上等同于 main() 或 WinMain()。
        /// </summary>
        
        DispatcherTimer Timer;
        public bool isFullScreen = false;
        public int NavViewPaneMode = 0;
        public Type ContentPage = typeof(Clock);

        public int StopWatch_IsStart = 0; //0关 1开 2暂停
        public long StopWatch_StartTick = 0;
        public long StopWatch_PauseTick = 0;
        public string StopWatch_Flags = "";

        public int Timer_IsStart = 0; //0关 1开 2暂停
        public long Timer_StartTick = 0;
        public long Timer_PauseTick = 0;

        public int Timer_HourSet = 0;
        public int Timer_MinuteSet = 0;
        public int Timer_SecondSet = 0;

        public App()
        {
            this.InitializeComponent();
            this.Suspending += OnSuspending;

            Timer = new DispatcherTimer();
            Timer.Interval = new TimeSpan(0, 0, 0, 1, 0);
            Timer.Tick += Timer_Tick;
            Timer.Start();
        }

        private async void Timer_Tick(object sender, object e)
        {
            ApplicationDataContainer LocalSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            Windows.Storage.ApplicationDataCompositeValue App_Theme = (ApplicationDataCompositeValue)LocalSettings.Values["App_Theme"];
            int ThemeSelected = 0;
            if (App_Theme != null)
            {
                ThemeSelected = (int)App_Theme["App_Theme"];
            }
            if (ThemeSelected == 0) 
            {
                ApplicationViewTitleBar TitleBar = ApplicationView.GetForCurrentView().TitleBar;
                if (App.Current.RequestedTheme == ApplicationTheme.Light)
                {
                    TitleBar.ButtonForegroundColor = Colors.Black;
                    TitleBar.ButtonHoverForegroundColor = Colors.Black;
                    TitleBar.ButtonPressedForegroundColor = Colors.Black;
                    TitleBar.ButtonInactiveForegroundColor = Colors.Gray;
                    TitleBar.ButtonHoverBackgroundColor = Colors.White;
                    TitleBar.ButtonPressedBackgroundColor = Colors.LightGray;
                }
                else if (App.Current.RequestedTheme == ApplicationTheme.Dark)
                {
                    TitleBar.ButtonForegroundColor = Colors.White;
                    TitleBar.ButtonHoverForegroundColor = Colors.White;
                    TitleBar.ButtonPressedForegroundColor = Colors.White;
                    TitleBar.ButtonInactiveForegroundColor = Colors.DarkGray;
                    TitleBar.ButtonHoverBackgroundColor = Colors.Black;
                    TitleBar.ButtonPressedBackgroundColor = Colors.Gray;
                }
            }

            if ((Application.Current as App).Timer_IsStart == 1)
            {
                long NowTicks = ((Application.Current as App).Timer_StartTick - DateTime.Now.Ticks / 10000);

                if (NowTicks <= 0)
                {
                    NowTicks = 0;
                }

                if (NowTicks == 0)
                {
                    (Application.Current as App).Timer_IsStart = 0;
                    (Application.Current as App).Timer_StartTick = 0;
                    (Application.Current as App).Timer_PauseTick = 0;

                    LocalSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
                    Windows.Storage.ApplicationDataCompositeValue Timer_Status = new Windows.Storage.ApplicationDataCompositeValue();
                    Timer_Status["IsStart"] = (Application.Current as App).Timer_IsStart;
                    Timer_Status["StartTick"] = (Application.Current as App).Timer_StartTick;
                    Timer_Status["PauseTick"] = (Application.Current as App).Timer_PauseTick;
                    LocalSettings.Values["Timer_Status"] = Timer_Status;

                    bool TimerNotification = true;
                    Windows.Storage.ApplicationDataCompositeValue TimerNotifiSwitch = (ApplicationDataCompositeValue)LocalSettings.Values["Timer_NotifiSwitch"];
                    if (TimerNotifiSwitch != null)
                    {
                        TimerNotification = (bool)TimerNotifiSwitch["Timer_Switch"];
                    }

                    string TimerGoOffTimeNow = DateTime.Now.ToLongTimeString().ToString();
                    if (TimerNotification == true)
                    {
                        new ToastContentBuilder()
                        .AddArgument("Action", "Timer")
                        .AddText("计时器操作完成")
                        .AddText(TimerGoOffTimeNow)
                        .Show();
                    }

                    ContentDialog Dialog = new ContentDialog();
                    Dialog.Title = "计时器操作完成";
                    Dialog.Content = TimerGoOffTimeNow;
                    Dialog.CloseButtonText = "确定";
                    Dialog.DefaultButton = ContentDialogButton.Close;

                    var result = await Dialog.ShowAsync();
                }
            }
        }

        /// <summary>
        /// 在应用程序由最终用户正常启动时进行调用。
        /// 将在启动应用程序以打开特定文件等情况下使用。
        /// </summary>
        /// <param name="e">有关启动请求和过程的详细信息。</param>
        Frame RootFrame;
        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
            RootFrame = Window.Current.Content as Frame;

            // 不要在窗口已包含内容时重复应用程序初始化，
            // 只需确保窗口处于活动状态
            if (RootFrame == null)
            {
                // 创建要充当导航上下文的框架，并导航到第一页
                RootFrame = new Frame();

                GetSettings();

                RootFrame.NavigationFailed += OnNavigationFailed;

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    //TODO: 从之前挂起的应用程序加载状态
                }

                // 将框架放在当前窗口中
                Window.Current.Content = RootFrame;
            }

            if (e.PrelaunchActivated == false)
            {
                if (RootFrame.Content == null)
                {
                    // 当导航堆栈尚未还原时，导航到第一页，
                    // 并通过将所需信息作为导航参数传入来配置
                    // 参数
                    RootFrame.Navigate(typeof(MainPage), e.Arguments);
                }
                // 确保当前窗口处于活动状态
                Window.Current.Activate();
            }
        }

        /// <summary>
        /// 导航到特定页失败时调用
        /// </summary>
        ///<param name="sender">导航失败的框架</param>
        ///<param name="e">有关导航失败的详细信息</param>
        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        /// <summary>
        /// 在将要挂起应用程序执行时调用。  在不知道应用程序
        /// 无需知道应用程序会被终止还是会恢复，
        /// 并让内存内容保持不变。
        /// </summary>
        /// <param name="sender">挂起的请求的源。</param>
        /// <param name="e">有关挂起请求的详细信息。</param>
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            //TODO: 保存应用程序状态并停止任何后台活动
            deferral.Complete();
        }

        public void GetSettings()
        {
            ApplicationDataContainer LocalSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            Windows.Storage.ApplicationDataCompositeValue App_Theme = (ApplicationDataCompositeValue)LocalSettings.Values["App_Theme"];
            if (App_Theme != null)
            {
                int ThemeSelected = (int)App_Theme["App_Theme"];
                if (ThemeSelected == 0)
                {
                    RootFrame.RequestedTheme = ElementTheme.Default;
                    ApplicationViewTitleBar TitleBar = ApplicationView.GetForCurrentView().TitleBar;
                    if (App.Current.RequestedTheme == ApplicationTheme.Light)
                    {
                        TitleBar.ButtonForegroundColor = Colors.Black;
                        TitleBar.ButtonHoverForegroundColor = Colors.Black;
                        TitleBar.ButtonPressedForegroundColor = Colors.Black;
                        TitleBar.ButtonInactiveForegroundColor = Colors.Gray;
                        TitleBar.ButtonHoverBackgroundColor = Colors.White;
                        TitleBar.ButtonPressedBackgroundColor = Colors.LightGray;
                    }
                    else if (App.Current.RequestedTheme == ApplicationTheme.Dark)
                    {
                        TitleBar.ButtonForegroundColor = Colors.White;
                        TitleBar.ButtonHoverForegroundColor = Colors.White;
                        TitleBar.ButtonPressedForegroundColor = Colors.White;
                        TitleBar.ButtonInactiveForegroundColor = Colors.DarkGray;
                        TitleBar.ButtonHoverBackgroundColor = Colors.Black;
                        TitleBar.ButtonPressedBackgroundColor = Colors.Gray;
                    }
                }
                else if (ThemeSelected == 1)
                {
                    RootFrame.RequestedTheme = ElementTheme.Light;
                    ApplicationViewTitleBar TitleBar = ApplicationView.GetForCurrentView().TitleBar;
                    TitleBar.ButtonForegroundColor = Colors.Black;
                    TitleBar.ButtonHoverForegroundColor = Colors.Black;
                    TitleBar.ButtonPressedForegroundColor = Colors.Black;
                    TitleBar.ButtonInactiveForegroundColor = Colors.Gray;
                    TitleBar.ButtonHoverBackgroundColor = Colors.White;
                    TitleBar.ButtonPressedBackgroundColor = Colors.LightGray;
                }
                else if (ThemeSelected == 2)
                {
                    RootFrame.RequestedTheme = ElementTheme.Dark;
                    ApplicationViewTitleBar TitleBar = ApplicationView.GetForCurrentView().TitleBar;
                    TitleBar.ButtonForegroundColor = Colors.White;
                    TitleBar.ButtonHoverForegroundColor = Colors.White;
                    TitleBar.ButtonPressedForegroundColor = Colors.White;
                    TitleBar.ButtonInactiveForegroundColor = Colors.DarkGray;
                    TitleBar.ButtonHoverBackgroundColor = Colors.Black;
                    TitleBar.ButtonPressedBackgroundColor = Colors.Gray;
                }
            }

            Windows.Storage.ApplicationDataCompositeValue NavView_PaneMode = (ApplicationDataCompositeValue)LocalSettings.Values["PaneMode"];
            if (NavView_PaneMode != null)
            {
                NavViewPaneMode = (int)NavView_PaneMode["PaneMode"];
            }

            String LocalValue = LocalSettings.Values["StopWatch_Status"] as string;
            Windows.Storage.ApplicationDataCompositeValue StopWatch_Status = (ApplicationDataCompositeValue)LocalSettings.Values["StopWatch_Status"];
            if (StopWatch_Status != null)
            {
                StopWatch_IsStart = (int)StopWatch_Status["IsStart"];
                StopWatch_StartTick = (long)StopWatch_Status["StartTick"];
                StopWatch_PauseTick = (long)StopWatch_Status["PauseTick"];
                StopWatch_Flags = StopWatch_Status["Flags"] as string;
            }

            LocalValue = LocalSettings.Values["Timer_Status"] as string;
            Windows.Storage.ApplicationDataCompositeValue Timer_Status = (ApplicationDataCompositeValue)LocalSettings.Values["Timer_Status"];
            if (Timer_Status != null)
            {
                Timer_IsStart = (int)Timer_Status["IsStart"];
                Timer_StartTick = (long)Timer_Status["StartTick"];
                Timer_PauseTick = (long)Timer_Status["PauseTick"];
            }
            Windows.Storage.ApplicationDataCompositeValue Timer_Set = (ApplicationDataCompositeValue)LocalSettings.Values["Timer_Set"];
            if (Timer_Set != null)
            {
                Timer_HourSet = (int)Timer_Set["Hour"];
                Timer_MinuteSet = (int)Timer_Set["Minute"];
                Timer_SecondSet = (int)Timer_Set["Second"];
            }
        }
    }
}
