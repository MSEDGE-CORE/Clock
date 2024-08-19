using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
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
using Windows.UI.Xaml.Navigation;

namespace Clock
{
    public sealed partial class Settings : Page
    {
        public Settings()
        {
            this.InitializeComponent();
            TextVersion.Text = string.Format("名称：时钟\n版本：{0}.{1}.{2}", Package.Current.Id.Version.Major, Package.Current.Id.Version.Minor, Package.Current.Id.Version.Build);
            Theme_Selection.SelectedIndex = (int)(Application.Current as App).LocalSettings.Values["Theme"];
        }

        private void Theme_Selection_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplicationViewTitleBar TitleBar = ApplicationView.GetForCurrentView().TitleBar;
            if (Theme_Selection.SelectedIndex == 0)
            {
                (Application.Current as App).RootFrame.RequestedTheme = ElementTheme.Default;
                TitleBar.ButtonForegroundColor = default;
                TitleBar.ButtonHoverForegroundColor = default;
                TitleBar.ButtonPressedForegroundColor = default;
                TitleBar.ButtonInactiveForegroundColor = default;
                TitleBar.ButtonHoverBackgroundColor = default;
                TitleBar.ButtonPressedBackgroundColor = default;
            }
            else if (Theme_Selection.SelectedIndex == 1)
            {
                (Application.Current as App).RootFrame.RequestedTheme = ElementTheme.Light;
                TitleBar.ButtonForegroundColor = Colors.Black;
                TitleBar.ButtonHoverForegroundColor = Colors.Black;
                TitleBar.ButtonPressedForegroundColor = Colors.Black;
                TitleBar.ButtonHoverBackgroundColor = Colors.White;
                TitleBar.ButtonPressedBackgroundColor = Colors.LightGray;
            }
            else if (Theme_Selection.SelectedIndex == 2)
            {
                (Application.Current as App).RootFrame.RequestedTheme = ElementTheme.Dark;
                TitleBar.ButtonForegroundColor = Colors.White;
                TitleBar.ButtonHoverForegroundColor = Colors.White;
                TitleBar.ButtonPressedForegroundColor = Colors.White;
                TitleBar.ButtonInactiveForegroundColor = Colors.DarkGray;
                TitleBar.ButtonHoverBackgroundColor = Colors.Black;
                TitleBar.ButtonPressedBackgroundColor = Colors.Gray;
            }

            (Application.Current as App).LocalSettings.Values["Theme"] = Theme_Selection.SelectedIndex;
        }
    }
}
