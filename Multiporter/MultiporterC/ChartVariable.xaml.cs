using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
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
    public sealed partial class ChartVariable : ContentDialog, INotifyPropertyChanged
    {
        public int iI;
        public int iD;
        private string independentType;
        public string IndependentType
        {
            get
            {
                if (independentType == null) { return "Choose a Variable"; }
                else { return independentType; }
            }
            set
            {
                independentType = value;
                OnPropertyChanged("IndependentType");
                OnPropertyChanged("IF");
            }
        }
        private string dependentType;
        public string DependentType
        {
            get
            {
                if (dependentType == null) { return "Choose a Variable";  }
                else { return dependentType; }
            }
            set
            {
                dependentType = value;
                OnPropertyChanged("DependentType");
                OnPropertyChanged("DF");
            }
        }
        public SolidColorBrush IF
        {
            get
            {
                if (independentType == null) { return new SolidColorBrush(Color.FromArgb(0xFF, 0x70, 0x70, 0x70)); }
                else { return new SolidColorBrush(Colors.Black); }
            }
        }
        public SolidColorBrush DF
        {
            get
            {
                if (dependentType == null) { return new SolidColorBrush(Color.FromArgb(0xFF, 0x70, 0x70, 0x70)); }
                else { return new SolidColorBrush(Colors.Black); }
            }
        }
        public List<VariableNode> Dependent { get; set; }
        public List<VariableNode> Independent { get; set; }

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        public ChartVariable(List<VariableNode> dependent, List<VariableNode> independent)
        {
            this.InitializeComponent();
            Independent = independent;
            Dependent = dependent;
        }

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }

        private void Independent_Click(object sender, RoutedEventArgs e)
        {
            Button b = sender as Button;
            MenuFlyout menu = new MenuFlyout();
            for (int i = 0; i < Independent.Count; i++)
            {
                MenuFlyoutItem item = new MenuFlyoutItem()
                {
                    Text = Independent[i].Measurement_Name,
                    FontSize = 16,
                    Tag = i
                };
                item.Click += delegate (object s, RoutedEventArgs arg)
                {
                    iI = (int)((MenuFlyoutItem)s).Tag;
                    IndependentType = ((MenuFlyoutItem)s).Text;
                };
                menu.Items.Add(item);
            }
            menu.ShowAt(b);
        }

        private void Dependent_Click(object sender, RoutedEventArgs e)
        {
            Button b = sender as Button;
            MenuFlyout menu = new MenuFlyout();
            for (int i = 0; i < Dependent.Count; i++)
            {
                MenuFlyoutItem item = new MenuFlyoutItem()
                {
                    Text = Dependent[i].Measurement_Name,
                    FontSize = 16,
                    Tag = i
                };
                item.Click += delegate (object s, RoutedEventArgs arg)
                {
                    iD = (int)((MenuFlyoutItem)s).Tag;
                    DependentType = ((MenuFlyoutItem)s).Text;
                };
                menu.Items.Add(item);
            }
            menu.ShowAt(b);
        }
    }
}
