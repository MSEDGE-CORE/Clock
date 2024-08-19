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

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace Clock
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class EditTimerDialog : Page
    {
        public EditTimerDialog()
        {
            this.InitializeComponent();

            ApplicationDataContainer LocalSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            Windows.Storage.ApplicationDataCompositeValue Timer_Set = (ApplicationDataCompositeValue)LocalSettings.Values["Timer_Set"];
            if (Timer_Set != null)
            {
                Timer_HourSet.SelectedIndex = (int)Timer_Set["Hour"];
                Timer_MinuteSet.SelectedIndex = (int)Timer_Set["Minute"];
                Timer_SecondSet.SelectedIndex = (int)Timer_Set["Second"];
            }
        }

        private void Timer_HourChanged(object sender, RoutedEventArgs e)
        {
            ApplicationDataContainer LocalSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            Windows.Storage.ApplicationDataCompositeValue Timer_Set = new Windows.Storage.ApplicationDataCompositeValue();
            Timer_Set["Hour"] = Timer_HourSet.SelectedIndex;
            Timer_Set["Minute"] = Timer_MinuteSet.SelectedIndex;
            Timer_Set["Second"] = Timer_SecondSet.SelectedIndex;
            LocalSettings.Values["Timer_Set"] = Timer_Set;
        }
        private void Timer_MinuteChanged(object sender, RoutedEventArgs e)
        {
            ApplicationDataContainer LocalSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            Windows.Storage.ApplicationDataCompositeValue Timer_Set = new Windows.Storage.ApplicationDataCompositeValue();
            Timer_Set["Hour"] = Timer_HourSet.SelectedIndex;
            Timer_Set["Minute"] = Timer_MinuteSet.SelectedIndex;
            Timer_Set["Second"] = Timer_SecondSet.SelectedIndex;
            LocalSettings.Values["Timer_Set"] = Timer_Set;
        }
        private void Timer_SecondChanged(object sender, RoutedEventArgs e)
        {
            ApplicationDataContainer LocalSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            Windows.Storage.ApplicationDataCompositeValue Timer_Set = new Windows.Storage.ApplicationDataCompositeValue();
            Timer_Set["Hour"] = Timer_HourSet.SelectedIndex;
            Timer_Set["Minute"] = Timer_MinuteSet.SelectedIndex;
            Timer_Set["Second"] = Timer_SecondSet.SelectedIndex;
            LocalSettings.Values["Timer_Set"] = Timer_Set;
        }

        private void Hour_Loaded(object sender, RoutedEventArgs e)
        {
            for(int i = 0;i <= 99; i++)
            {
                (sender as ComboBox).Items.Add(i);
            }
        }
        private void Minute_Loaded(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i <= 59; i++)
            {
                (sender as ComboBox).Items.Add(i);
            }
        }
        private void Second_Loaded(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i <= 59; i++)
            {
                (sender as ComboBox).Items.Add(i);
            }
        }

        private void Frame_Loaded(object sender, RoutedEventArgs e)
        {
            EntranceAnimation.Begin();
        }
    }
}
