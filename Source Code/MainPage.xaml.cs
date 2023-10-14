using System;
using System.Collections.Generic;
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
using Windows.Devices.PointOfService;
using System.Threading;
using Windows.UI.Core;

// https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x804 上介绍了“空白页”项模板

namespace Clock
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        DispatcherTimer Timer;
        Type ContentPage = (Application.Current as App).ContentPage;

        public MainPage()
        {
            this.InitializeComponent();

            ContentPage = (Application.Current as App).ContentPage;

            var CoreTitleBar = CoreApplication.GetCurrentView().TitleBar;
            CoreTitleBar.ExtendViewIntoTitleBar = true;
            ApplicationViewTitleBar TitleBar = ApplicationView.GetForCurrentView().TitleBar;
            TitleBar.ButtonBackgroundColor = Colors.Transparent;
            TitleBar.ButtonInactiveBackgroundColor = Colors.Transparent;
            Window.Current.SetTitleBar(AppTitleBar);

            Timer = new DispatcherTimer();
            Timer.Interval = new TimeSpan(0, 0, 0, 0, 100);
            Timer.Tick += Timer_Tick;
            Timer.Start();
        }

        private void Timer_Tick(object sender, object e)
        {
            if (ActualWidth <= 640 && NavView.PaneDisplayMode == Microsoft.UI.Xaml.Controls.NavigationViewPaneDisplayMode.Auto)
            {
                AppIcon.Margin = new Thickness(16, 8, 0, 0);
            }
            else
            {
                AppIcon.Margin = new Thickness(16, 12, 0, 0);
            }

            if ((Application.Current as App).NavViewPaneMode == 0 && NavView.PaneDisplayMode != Microsoft.UI.Xaml.Controls.NavigationViewPaneDisplayMode.Auto)
            {
                NavView.PaneDisplayMode = Microsoft.UI.Xaml.Controls.NavigationViewPaneDisplayMode.Auto;
                WindowControl.Margin = new Thickness(0, 32, 0, 0);
            }
            else if((Application.Current as App).NavViewPaneMode == 1 && NavView.PaneDisplayMode != Microsoft.UI.Xaml.Controls.NavigationViewPaneDisplayMode.Top)
            {
                NavView.PaneDisplayMode = Microsoft.UI.Xaml.Controls.NavigationViewPaneDisplayMode.Top;
                WindowControl.Margin = new Thickness(0, 80, 0, 0);
            }
        }

        private double NavViewCompactModeThresholdWidth
        { 
            get 
            { 
                return NavView.CompactModeThresholdWidth; 
            } 
        }

        private void ContentFrame_NavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        private void NavView_Loaded(object sender, RoutedEventArgs e)
        {
            if ((Application.Current as App).NavViewPaneMode == 0)
            {
                NavView.PaneDisplayMode = Microsoft.UI.Xaml.Controls.NavigationViewPaneDisplayMode.Auto;
                WindowControl.Margin = new Thickness(0, 32, 0, 0);
            }
            else if ((Application.Current as App).NavViewPaneMode == 1)
            {
                NavView.PaneDisplayMode = Microsoft.UI.Xaml.Controls.NavigationViewPaneDisplayMode.Top;
                WindowControl.Margin = new Thickness(0, 80, 0, 0);
            }

            ContentFrame.Navigated += On_Navigated;
            NavView.SelectedItem = NavView.MenuItems[0];
            NavView_Navigate(ContentPage, new SuppressNavigationTransitionInfo());
        }

        private void NavView_ItemInvoked(Microsoft.UI.Xaml.Controls.NavigationView sender, Microsoft.UI.Xaml.Controls.NavigationViewItemInvokedEventArgs args)
        {
            if (args.IsSettingsInvoked == true)
            {
                NavView_Navigate(typeof(Settings), args.RecommendedNavigationTransitionInfo);
            }
            else if (args.InvokedItemContainer != null)
            {
                Type NavPageType = Type.GetType(args.InvokedItemContainer.Tag.ToString());
                NavView_Navigate(NavPageType, args.RecommendedNavigationTransitionInfo);
            }
        }

        private void NavView_SelectionChanged(Microsoft.UI.Xaml.Controls.NavigationView sender, Microsoft.UI.Xaml.Controls.NavigationViewSelectionChangedEventArgs args)
        {
            if (args.IsSettingsSelected == true)
            {
                NavView_Navigate(typeof(Settings), args.RecommendedNavigationTransitionInfo);
            }
            else if (args.SelectedItemContainer != null)
            {
                Type NavPageType = Type.GetType(args.SelectedItemContainer.Tag.ToString());
                NavView_Navigate(NavPageType, args.RecommendedNavigationTransitionInfo);
            }
        }

        private void NavView_Navigate(Type NavPageType, NavigationTransitionInfo transitionInfo)
        {
            Type PreNavPageType = ContentFrame.CurrentSourcePageType;

            if (NavPageType != null && !Type.Equals(PreNavPageType, NavPageType))
            {
                ContentFrame.Navigate(NavPageType, null, new EntranceNavigationTransitionInfo());

                if ((NavPageType == typeof(Settings)))
                {
                    WindowControl.Visibility = Visibility.Collapsed;
                }
                else
                {
                    WindowControl.Visibility = Visibility.Visible;
                }

                if (!(NavPageType == typeof(Clock)))
                {
                    Compact.Visibility = Visibility.Collapsed;
                }
                else
                {
                    Compact.Visibility = Visibility.Visible;
                }
            }
        }

        private void NavView_BackRequested(Microsoft.UI.Xaml.Controls.NavigationView sender, Microsoft.UI.Xaml.Controls.NavigationViewBackRequestedEventArgs args)
        {
            TryGoBack();

            if ((ContentFrame.CurrentSourcePageType == typeof(Settings)))
            {
                WindowControl.Visibility = Visibility.Collapsed;
            }
            else
            {
                WindowControl.Visibility = Visibility.Visible;
            }

            if (!(ContentFrame.CurrentSourcePageType == typeof(Clock)))
            {
                Compact.Visibility = Visibility.Collapsed;
            }
            else
            {
                Compact.Visibility = Visibility.Visible;
            }
        }

        private bool TryGoBack()
        {
            if (!ContentFrame.CanGoBack)
            {
                return false;
            }

            if (NavView.IsPaneOpen && (NavView.DisplayMode == Microsoft.UI.Xaml.Controls.NavigationViewDisplayMode.Compact || NavView.DisplayMode == Microsoft.UI.Xaml.Controls.NavigationViewDisplayMode.Minimal))
            {
                return false;
            }

            ContentFrame.GoBack();
            return true;
        }

        private void On_Navigated(object sender, NavigationEventArgs e)
        {
            NavView.IsBackEnabled = ContentFrame.CanGoBack;

            if (ContentFrame.SourcePageType == typeof(Settings))
            {
                NavView.SelectedItem = (Microsoft.UI.Xaml.Controls.NavigationViewItem)NavView.SettingsItem;

                NavView.AlwaysShowHeader = true;
                NavView.Header = "设置";
            }
            else if (ContentFrame.SourcePageType != null)
            {
                NavView.SelectedItem = NavView.MenuItems.OfType<Microsoft.UI.Xaml.Controls.NavigationViewItem>().First(i => i.Tag.Equals(ContentFrame.SourcePageType.FullName.ToString()));

                NavView.AlwaysShowHeader = false;
                NavView.Header = ((Microsoft.UI.Xaml.Controls.NavigationViewItem)NavView.SelectedItem)?.Content?.ToString();
                NavView.AlwaysShowHeader = false;

            }
        }

        private async void Compact_Click(object sender, RoutedEventArgs e)
        {
            (Application.Current as App).isFullScreen = true;
            (Application.Current as App).ContentPage = ContentPage = ContentFrame.SourcePageType;
            Frame.Navigate(ContentFrame.SourcePageType, null, new SuppressNavigationTransitionInfo());

            ApplicationView view = ApplicationView.GetForCurrentView();
            if (view.ViewMode != ApplicationViewMode.CompactOverlay)
            {
                await view.TryEnterViewModeAsync(ApplicationViewMode.CompactOverlay);
            }
        }

        private void FullScreen_Click(object sender, RoutedEventArgs e)
        {
            (Application.Current as App).isFullScreen = true;
            (Application.Current as App).ContentPage = ContentPage = ContentFrame.SourcePageType;
            Frame.Navigate(ContentFrame.SourcePageType, null, new SuppressNavigationTransitionInfo());

            ApplicationView view = ApplicationView.GetForCurrentView();
            if (!view.IsFullScreenMode)
            {
                view.TryEnterFullScreenMode();
            }
        }

        private async void NewWindow_Click(object sender, RoutedEventArgs e)
        {
            (Application.Current as App).ContentPage = ContentPage = ContentFrame.SourcePageType;
            (Application.Current as App).isFullScreen = true;

            CoreApplicationView NewView = CoreApplication.CreateNewView();
            int NewViewId = 0;
            await NewView.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                Frame frame = new Frame();
                frame.Navigate(ContentPage, null);
                Window.Current.Content = frame;
                Window.Current.Activate();

                NewViewId = ApplicationView.GetForCurrentView().Id;
                (Application.Current as App).isFullScreen = false;
            });
            bool ViewShown = await ApplicationViewSwitcher.TryShowAsStandaloneAsync(NewViewId);
        }
    }
}
