using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Xml.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.System;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace Clock
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class Settings : Page
    {
        public Settings()
        {
            this.InitializeComponent();

            ApplicationDataContainer LocalSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            Windows.Storage.ApplicationDataCompositeValue App_Theme = (ApplicationDataCompositeValue)LocalSettings.Values["App_Theme"];
            if (App_Theme != null)
            {
                Theme_Selection.SelectedIndex = (int)App_Theme["App_Theme"];
            }
            else
            {
                Theme_Selection.SelectedIndex = 0;
            }
            Windows.Storage.ApplicationDataCompositeValue TimerNotifiSwitch = (ApplicationDataCompositeValue)LocalSettings.Values["Timer_NotifiSwitch"];
            if (TimerNotifiSwitch != null)
            {
                TimerNotification_Switch.IsOn = (bool)TimerNotifiSwitch["Timer_Switch"];
            }
            else
            {
                TimerNotification_Switch.IsOn = true;
            }
            NavView_Selection.SelectedIndex = (Application.Current as App).NavViewPaneMode;
        }

        private void Theme_SelectionChanged(object sender, RoutedEventArgs e)
        {
            int ThemeSelected = Theme_Selection.SelectedIndex;
            if(ThemeSelected == 0)
            {
                (Window.Current.Content as Frame).RequestedTheme = ElementTheme.Default;
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
                (Window.Current.Content as Frame).RequestedTheme = ElementTheme.Light;
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
                (Window.Current.Content as Frame).RequestedTheme = ElementTheme.Dark;
                ApplicationViewTitleBar TitleBar = ApplicationView.GetForCurrentView().TitleBar;
                TitleBar.ButtonForegroundColor = Colors.White;
                TitleBar.ButtonHoverForegroundColor = Colors.White;
                TitleBar.ButtonPressedForegroundColor = Colors.White;
                TitleBar.ButtonInactiveForegroundColor = Colors.DarkGray;
                TitleBar.ButtonHoverBackgroundColor = Colors.Black;
                TitleBar.ButtonPressedBackgroundColor = Colors.Gray;
            }

            ApplicationDataContainer LocalSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            Windows.Storage.ApplicationDataCompositeValue App_Theme = new Windows.Storage.ApplicationDataCompositeValue();
            App_Theme["App_Theme"] = Theme_Selection.SelectedIndex;
            LocalSettings.Values["App_Theme"] = App_Theme;
        }

        private void TimerNotifi_Toggled(object sender, RoutedEventArgs e)
        {
            ApplicationDataContainer LocalSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            Windows.Storage.ApplicationDataCompositeValue TimerNotifiSwitch = new Windows.Storage.ApplicationDataCompositeValue();
            TimerNotifiSwitch["Timer_Switch"] = TimerNotification_Switch.IsOn;
            LocalSettings.Values["Timer_NotifiSwitch"] = TimerNotifiSwitch;
        }

        private void NavView_SelectionChanged(object sender, RoutedEventArgs e)
        {
            (Application.Current as App).NavViewPaneMode = NavView_Selection.SelectedIndex;

            ApplicationDataContainer LocalSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            Windows.Storage.ApplicationDataCompositeValue NavView_PaneMode = new Windows.Storage.ApplicationDataCompositeValue();
            NavView_PaneMode["PaneMode"] = NavView_Selection.SelectedIndex;
            LocalSettings.Values["PaneMode"] = NavView_PaneMode;
        }
    }
}
