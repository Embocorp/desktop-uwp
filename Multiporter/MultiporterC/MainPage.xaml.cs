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
using System.Diagnostics;
using Windows.Foundation.Metadata;
using Windows.UI.ViewManagement;
using Windows.UI;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace MultiporterC
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            if (ApiInformation.IsTypePresent("Windows.UI.ViewManagement.ApplicationView"))
            {
                var titleBar = ApplicationView.GetForCurrentView().TitleBar;
                if (titleBar != null)
                {
                    Color mBlue = Color.FromArgb(0xFF, 0x00, 0x71, 0xC1);
                    titleBar.ButtonBackgroundColor = mBlue;
                    titleBar.ButtonForegroundColor = Colors.White;
                    titleBar.BackgroundColor = mBlue;
                    titleBar.ForegroundColor = Colors.White;
                }
            }
            CacheManagement.CheckLocalConnections(true);
            NavPanel.ExpandClick += new RoutedEventHandler(this.NavControl_Expand);
        }

        private void NavControl_Expand(object sender, RoutedEventArgs e)
        {
            MySplitView.IsPaneOpen = !MySplitView.IsPaneOpen;
        }

        private void ExpNav_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(Experiments));
        }
        
        private void DevNav_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(Devices));
        }

        private void Account_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(UserPage));
        }
    }
}
