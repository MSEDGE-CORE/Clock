using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace Clock
{
    public sealed partial class EditTimerDialog : Page
    {
        public EditTimerDialog()
        {
            this.InitializeComponent();

            Timer_HourSet.SelectedIndex = (Application.Current as App).Timer_HourSet;
            Timer_MinuteSet.SelectedIndex = (Application.Current as App).Timer_MinuteSet;
            Timer_SecondSet.SelectedIndex = (Application.Current as App).Timer_SecondSet;
        }

        private void Timer_HourChanged(object sender, RoutedEventArgs e)
        {
            (Application.Current as App).Timer_HourSet = Timer_HourSet.SelectedIndex;
        }
        private void Timer_MinuteChanged(object sender, RoutedEventArgs e)
        {
            (Application.Current as App).Timer_MinuteSet = Timer_MinuteSet.SelectedIndex;
        }
        private void Timer_SecondChanged(object sender, RoutedEventArgs e)
        {
            (Application.Current as App).Timer_SecondSet = Timer_SecondSet.SelectedIndex;
        }

        private void Hour_Loaded(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i <= 99; i++)
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
