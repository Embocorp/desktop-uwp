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

// The Content Dialog item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace MultiporterC
{
    public sealed partial class NewMeasurement : ContentDialog
    {
        public NewMeasurement()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty MeasurementProperty = DependencyProperty.Register(
            "MeasurementName", typeof(string), typeof(NewMeasurement), new PropertyMetadata(default(string)));

        public static readonly DependencyProperty UnitProperty = DependencyProperty.Register(
            "UnitName", typeof(string), typeof(NewMeasurement), new PropertyMetadata(default(string)));

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }

        public string MeasurementName
        {
            get { return (string)GetValue(MeasurementProperty); }
            set { SetValue(MeasurementProperty, value); }
        }
        public string UnitName
        {
            get { return (string)GetValue(UnitProperty); }
            set { SetValue(UnitProperty, value); }
        }
    }
}
