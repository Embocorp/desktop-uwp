using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using WinRTXamlToolkit.Controls.DataVisualization.Charting;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace MultiporterC
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Explorer : Page
    {
        private string ExperimentName;
        private string CreatedDate;
        private string Author;
        private bool Playing;
        private Experiment Exp;

        public Explorer()
        {
            this.InitializeComponent();
            NavPanel.ExpandClick += new RoutedEventHandler(this.NavControl_Expand);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            Experiment exp = e.Parameter as Experiment;
            Exp = exp;
            ExperimentName = exp.Name;
            ExpName.Text = ExperimentName;
            CreatedDate = exp.Created;
            ExperimentCreated.Text = CreatedDate;
            Author = exp.Author;
            ExperimentAuthor.Text = Author;
            for (int i = 0; i < exp.Cards.Count; i++)
            {
                string cat = exp.Cards[i].Category.Replace(" ", "");
                ListView list = (ListView)FindName(cat + "Cards");
                list.Items.Add(exp.Cards[i]);
            }
        }

        private void NavControl_Expand(object sender, RoutedEventArgs e)
        {
            MySplitView.IsPaneOpen = !MySplitView.IsPaneOpen;
        }

        private async void Description_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            ContentDialog dialog = new ContentDialog()
            {
                Content = button.Tag,
                PrimaryButtonText = "OK"
            };
            await dialog.ShowAsync();
        }

        private async void New_Card_Click(object sender, RoutedEventArgs e)
        {
            if (Playing == true)
            {
                var messageDialog = new MessageDialog("You can't edit your experiment while it is running");
                messageDialog.Commands.Add(new UICommand("OK"));
                messageDialog.CancelCommandIndex = 0;
                await messageDialog.ShowAsync();
            }
            else
            {
                MenuFlyout m = new MenuFlyout();
                NodeType[] items;
                string sel = ((PivotItem)RootPivot.SelectedItem).Header as string;
                items = ExperimentNodeChooser.GetNodeList(sel);
                foreach (NodeType i in items)
                {
                    MenuFlyoutItem mi = new MenuFlyoutItem()
                    {
                        Text = i.Name,
                        FontSize = 20,
                        Tag = i.Member
                    };
                    mi.Click += Create_New_Card;
                    m.Items.Add(mi);
                }
                m.ShowAt((Button)sender);
            }
        }   

        public async void Create_New_Card(object sender, RoutedEventArgs e)
        {
            ExperimentNode obj = ((MenuFlyoutItem)sender).Tag as ExperimentNode;
            string sel = ((PivotItem)RootPivot.SelectedItem).Header as string;
            if (obj.GetType() == typeof(DataChartNode))
            {
                DataChartNode chart = ((DataChartNode)obj);
                List<VariableNode> dVars = Exp.GetVariables(VariableNode.VariableType.Dependent);
                List<VariableNode> iVars = Exp.GetVariables(VariableNode.VariableType.Independent);
                
                string title = "";
                if (dVars.Count > 0 && iVars.Count > 0)
                {
                    ChartVariable chartDialog = new ChartVariable(dVars, iVars);
                    var result = await chartDialog.ShowAsync();
                    if (result == ContentDialogResult.Primary)
                    {
                        int iD = chartDialog.iD;
                        int iI = chartDialog.iI;
                        title = iVars[iI].Measurement_Name + " (" + iVars[iI].Unit_Name + ") vs " +
                        dVars[iD].Measurement_Name + " (" + dVars[iD].Unit_Name + ")";
                        chart.Independent = iVars[iI];
                        chart.Dependent = dVars[iD];
                        chart.Title = title;
                        ListView list = (ListView)FindName(sel.Replace(" ", "") + "Cards");
                        list.Items.Add(obj);
                        Exp.AddNode(obj);
                        Exp.Save();
                    }
                }
                else
                {
                    ContentDialog dialog = new ContentDialog()
                    {
                        Title = "Please specify experiment variables",
                        Content = "To create a chart, experiment variables must be created in" +
                        "the Problem Tab.  Go to the tab and specify at least one Independent and " +
                        "Dependent Variable.",
                        PrimaryButtonText="OK"
                    };
                    await dialog.ShowAsync();
                }
            }
            else
            {
                Exp.AddNode(obj);
                ListView list = (ListView)FindName(sel.Replace(" ", "") + "Cards");
                list.Items.Add(obj);
                Exp.Save();
            }
        }

        public void Delete_Card_Click (object sender, RoutedEventArgs e)
        {
            Button tar = (Button)sender;
            ExperimentNode remove = (ExperimentNode)tar.Tag;
            ListView list = (ListView)FindName(remove.Category.Replace(" ", "") + "Cards");
            list.Items.Remove(remove);
            Exp.RemoveNode(remove);
            Exp.Save();
        }

        public T GetAncestorOfType<T>(FrameworkElement child) where T : FrameworkElement
        {
            var parent = VisualTreeHelper.GetParent(child);
            if (parent != null && !(parent is T))
                return (T)GetAncestorOfType<T>((FrameworkElement)parent);
            return (T)parent;
        }

        public void Save_Card_Click(object sender, RoutedEventArgs e)
        {
            Exp.Save();
        }

        public async void Play_Click(object sender, RoutedEventArgs e)
        {
            if (Exp.Devices != null && Exp.Devices.Length > 0)
            {
                Button butt = sender as Button;
                MenuFlyout m = await Show_Experiment_Devices();
                m.ShowAt(butt);
                //Play_Experiment();
            }
            else
            {
                ContentDialog PlayDialog = new ContentDialog()
                {
                    Title = "No Connected Device",
                    Content = "Would you like to run the experiment manually?  " +
                    "Additionally, you can add a connected device to the experiment using the " +
                    "dropdown menu to the left.",
                    PrimaryButtonText = "Run Manually",
                    SecondaryButtonText = "Cancel"
                };
                PlayDialog.PrimaryButtonClick += delegate
                {
                    Run_Manual();
                    Playing = true;
                    Button butt = sender as Button;
                    ProgressRing ring = new ProgressRing()
                    {
                        Height = 25,
                        Width = 25,
                        IsActive = true
                    };
                    butt.Content = ring;
                    butt.Click -= Play_Click;
                    butt.Click += Abort_Experiment;
                };

                await PlayDialog.ShowAsync();
            }
        }

        private async Task<MenuFlyout> Show_Experiment_Devices()
        {
            Device[] c = Exp.Devices;
            List<Device> b = await CacheManagement.GetConnectedDevices();
            MenuFlyout m = new MenuFlyout();
            bool available = false;

            for (int i = 0; i < c.Length; i++)
            {
                c[i].Connected = false;
                for (int j = 0; j < b.Count; j++)
                {
                    if (c[i] == b[j])
                    {
                        c[i].Connected = true;
                    }
                }
            }
            if (c.Length > 0)
            {
                foreach (Device d in c)
                {
                    available = available || d.Connected;
                    MenuFlyoutItem mi = new MenuFlyoutItem()
                    {
                        Text = d.Name,
                        IsEnabled = d.Connected,
                        Tag = d
                    };
                    mi.Click += delegate
                    {
                        Play_Experiment((Device)this.Tag);
                    };
                    m.Items.Add(mi);
                }
                if (!available)
                {
                    MenuFlyoutItem mi = new MenuFlyoutItem()
                    {
                        Text = "Experiment Manually"
                    };
                    mi.Click += delegate
                    {
                        Run_Manual();
                    };
                    m.Items.Add(mi);
                }
            }
            else
            {
                MenuFlyoutItem placeholder = new MenuFlyoutItem()
                {
                    Text = "Run Manually"
                };
                m.Items.Add(placeholder);
            }
            return m;
        }

        public void Abort_Experiment(object sender, RoutedEventArgs e)
        {
            Button b = (Button)sender;
            Playing = false;
            b.Content = "\uE768";
            b.Click += Play_Click;
            b.Click -= Abort_Experiment;
        }

        public async void Run_Manual()
        {
            DataChartNode active = null;
            foreach (ExperimentNode node in Exp.Cards)
            {
                if (node.GetType() == typeof(DataChartNode))
                {
                    active = node as DataChartNode;
                }
            }
            for (double i = 0; i < 120; i+=0.5)
            {
                Debug.WriteLine("Playing");
                
                DataPoint d = await Get_DataPoints(i);
                Debug.WriteLine(d.X);
                active.Add_Data_Point(d);
                ListViewItem temp = (ListViewItem)DataAnalysisCards.ContainerFromItem(active);
                Chart c = FindByName("chart", temp) as Chart;

                if (active.Type == DataChartNode.ChartType.Line)
                {
                    ((LineSeries)c.Series[0]).ItemsSource = null;
                    ((LineSeries)c.Series[0]).ItemsSource = active.Data;
                }
                else if (active.Type == DataChartNode.ChartType.Bar)
                {
                    ((LineSeries)c.Series[0]).ItemsSource = null;
                    ((LineSeries)c.Series[0]).ItemsSource = active.Data;
                }
                else if (active.Type == DataChartNode.ChartType.Scatter)
                {
                    ((LineSeries)c.Series[0]).ItemsSource = null;
                    ((LineSeries)c.Series[0]).ItemsSource = active.Data;
                }
            }
        }

        private FrameworkElement FindByName(string name, FrameworkElement root)
        {
            Stack<FrameworkElement> tree = new Stack<FrameworkElement>();
            tree.Push(root);

            while (tree.Count > 0)
            {
                FrameworkElement current = tree.Pop();
                if (current.Name == name)
                    return current;

                int count = VisualTreeHelper.GetChildrenCount(current);
                for (int i = 0; i < count; ++i)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(current, i);
                    if (child is FrameworkElement)
                        tree.Push((FrameworkElement)child);
                }
            }

            return null;
        }

        public async void Play_Experiment (Device tool)
        {
            
            for (double i = 0; i < 120; i+=0.5)
            {
                DataChartNode active = null;
                foreach (ExperimentNode node in Exp.Cards)
                {
                    if (node.GetType() == typeof(DataChartNode))
                    {
                        active = node as DataChartNode;
                    }
                }
                Debug.WriteLine("Playing");
                
                DataPoint d = await Get_DataPoints(i);
                Debug.WriteLine(d.X);
                active.Add_Data_Point(d);               
            }
            
            Playing = true;
            ProgressRing ring = new ProgressRing()
            {
                Height = 25,
                Width = 25,
                IsActive = true
            };
            PlayExperimentButton.Content = ring;
            PlayExperimentButton.Click -= Play_Click;
            PlayExperimentButton.Click += Abort_Experiment;
        }

        public async Task<DataPoint> Get_DataPoints(int i)
        {
            await Task.Delay(1000);
            return new DataPoint(i, 0);
        }

        public async Task<DataPoint> Get_DataPoints(double prev)
        {
            await Task.Delay(500);
            return await Task.Run(() =>
            {
                Random rnd = new Random();
                double min = rnd.NextDouble() * 2;

                return new DataPoint(prev, min);
            });
        }

        public void Connect_Device_To_Experiment(Device device)
        {
            if (Exp.Devices == null)
            {
                Exp.Devices = new Device[] { device };
            }
            else
            {
                Device[] new_array = new Device[Exp.Devices.Length + 1];
                for (int i = 0; i < Exp.Devices.Length; i++)
                {
                    new_array[i] = Exp.Devices[i];
                }
                new_array[new_array.Length - 1] = device;
                Exp.Devices = new_array;
            }
            Exp.Save();
        }

        public async void GetConnectedDevices (object sender, RoutedEventArgs e)
        {
            List<Device> c = new List<Device>();
            c = await CacheManagement.GetConnectedDevices();
            MenuFlyout m = new MenuFlyout();
            Device[] already = Exp.Devices;

            if (already != null && c.Count > 0)
            {
                foreach (Device d in already)
                {
                    for (int i = 0; i < c.Count; i++)
                    {
                        if (d == c[i])
                        {
                            c.Remove(c[i]);
                        }
                    }
                }
            }

            if (c.Count > 0)
            {                
                for (int i = 0; i < c.Count; i++)
                {
                    MenuFlyoutItem mi = new MenuFlyoutItem()
                    {
                        Text = c[i].Name,
                        FontSize = 16,
                        Tag = c[i]
                    };
                    mi.Click += Connect_Device_To_Experiment_Click;
                    m.Items.Add(mi);
                }
            }
            else
            {
                MenuFlyoutItem mi = new MenuFlyoutItem()
                {
                    Text = "No Connected Devices",
                    FontSize = 16,
                    IsEnabled = false
                };
                m.Items.Add(mi);
            }
            m.ShowAt((Button)sender);
         }
        
        private void Connect_Device_To_Experiment_Click(object sender, RoutedEventArgs e)
        {
            MenuFlyoutItem b = (MenuFlyoutItem)sender;
            Connect_Device_To_Experiment((Device)b.Tag);
        }

        public void Experiment_Settings_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(ExperimentSetting), Exp);
        }

        private void Export_Click(object sender, RoutedEventArgs e)
        {
            MenuFlyout m = new MenuFlyout();
            MenuFlyoutItem mi1 = new MenuFlyoutItem()
            {
                Text = ".mport"
            };
            mi1.Click += delegate
            {
                Export_Experiment();
            };
            MenuFlyoutItem mi2 = new MenuFlyoutItem()
            {
                Text = ".pdf",
                IsEnabled = false
            };
            MenuFlyoutItem mi3 = new MenuFlyoutItem()
            {
                Text = ".docx",
                IsEnabled = false
            };
            MenuFlyoutItem mi4 = new MenuFlyoutItem()
            {
                Text = ".txt",
                IsEnabled = false
            };
            m.Items.Add(mi1);
            m.Items.Add(mi2);
            m.Items.Add(mi3);
            m.Items.Add(mi4);
            m.ShowAt((Button)sender);
        }

        public async void Export_Experiment()
        {
            ContentDialog resultDialog;
            var savePicker = new Windows.Storage.Pickers.FileSavePicker();
            savePicker.SuggestedStartLocation =
                Windows.Storage.Pickers.PickerLocationId.DocumentsLibrary;
            // Dropdown of file types the user can save the file as
            savePicker.FileTypeChoices.Add("Multiporter Experiment File (*.mport)", new List<string>() { ".mport" });
            // Default file name if the user does not type one in or select a file to replace
            savePicker.SuggestedFileName = Exp.Name;

            Windows.Storage.StorageFile file = await savePicker.PickSaveFileAsync();
            if (file != null)
            {
                // Prevent updates to the remote version of the file until
                // we finish making changes and call CompleteUpdatesAsync.
                Windows.Storage.CachedFileManager.DeferUpdates(file);
                string xml = Serialize.Xml_Serialize_Object<Experiment>(Exp);
                // write to file
                await Windows.Storage.FileIO.WriteTextAsync(file, xml);
                // Let Windows know that we're finished changing the file so
                // the other app can update the remote version of the file.
                // Completing updates may require Windows to ask for user input.
                Windows.Storage.Provider.FileUpdateStatus status =
                    await Windows.Storage.CachedFileManager.CompleteUpdatesAsync(file);
                if (status == Windows.Storage.Provider.FileUpdateStatus.Complete)
                {
                    resultDialog = new ContentDialog()
                    {
                        Title = "File saved successfully!",
                        PrimaryButtonText = "OK"
                    };
                }
                else
                {
                    resultDialog = new ContentDialog()
                    {
                        Title = "Oops... Something went wrong",
                        PrimaryButtonText = "OK"
                    };
                }
                await resultDialog.ShowAsync();
            }
            
        }

        public void Choose_Measurement(object sender, RoutedEventArgs e)
        {
            List<Measurement> measurements = new List<Measurement>();
            Device[] devices = Exp.Devices;
            MenuFlyout menu = new MenuFlyout();
            Button butt = (Button)sender;
            
            if (devices != null)
            {
                foreach (Device d in devices)
                {
                    foreach (Sensor s in d.Sensors)
                    {
                        foreach (Measurement m in s.Measurements)
                        {
                            measurements.Add(m);
                        }
                    }
                }
            }

            if (measurements.Count < 1)
            {
                MenuFlyoutItem item = new MenuFlyoutItem()
                {
                    Text = "No Connected Devices",
                    IsEnabled = false,
                    FontSize = 18
                };
                menu.Items.Add(item);
            }
            for (int i = 0; i < measurements.Count; i++)
            {
                MenuFlyoutItem item = new MenuFlyoutItem()
                {
                    Text = measurements[i].Name,
                    FontSize = 18,
                    Tag = measurements[i]
                };
                menu.Items.Add(item);
                item.Click += delegate (object s, RoutedEventArgs arg)
                {
                    VariableNode sel = butt.Tag as VariableNode;
                    sel.MeasureSource = (Measurement)((MenuFlyoutItem)s).Tag;
                    sel.UnitSource = -1;
                    Exp.Save();
                };
            }
            MenuFlyoutItem ok = new MenuFlyoutItem()
            {
                Text = "Create Custom Measurement",
                FontSize = 18
            };
            ok.Click += async delegate
            {
                Measurement m = await Create_Custom_Measurement();
                if (m != null)
                {
                    VariableNode sel = butt.Tag as VariableNode;
                    sel.MeasureSource = m;
                    sel.UnitSource = -1;
                    Exp.Save();
                }
            };
            menu.Items.Add(ok);
            menu.ShowAt(butt);
        }

        public void Choose_Unit(object sender, RoutedEventArgs e)
        {
            VariableNode variable = (VariableNode)((Button)sender).Tag;
            Measurement measure = variable.Measure;
            MenuFlyout menu = new MenuFlyout();
            Button parent = (Button)sender;

            for (int i = 0; i < measure.Unit.Length; i++)
            {
                MenuFlyoutItem item = new MenuFlyoutItem()
                {
                    Text = measure.Unit[i],
                    Tag = i, 
                    FontSize = 18
                };
                item.Click += delegate (object s, RoutedEventArgs args)
                {
                    variable.UnitSource = (int)((MenuFlyoutItem)s).Tag;
                    Exp.Save();
                };
                menu.Items.Add(item);
            }
            menu.ShowAt(parent);
        }

        public async Task<Measurement> Create_Custom_Measurement()
        {
            var dialog1 = new NewMeasurement();
            Measurement newMeasurement = new Measurement();
            var result = await dialog1.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                newMeasurement = new Measurement(dialog1.MeasurementName, new string[] { dialog1.UnitName });
                return newMeasurement;
            }
            else
            {
                return null;
            }
        }

        public void Quant_Type_Click (object sender, RoutedEventArgs e)
        {
            QuantitativeRelationshipNode.QuantType[] options = { QuantitativeRelationshipNode.QuantType.Correlation, QuantitativeRelationshipNode.QuantType.Function, QuantitativeRelationshipNode.QuantType.Statistical };
            MenuFlyoutItem sel = (MenuFlyoutItem)sender;
            QuantitativeRelationshipNode node = sel.DataContext as QuantitativeRelationshipNode;
            int index = Convert.ToInt32((string)sel.Tag);
            node.TypeSource = options[index];
            Exp.Save();
        }

        public void Choose_Subtype (object sender, RoutedEventArgs e)
        {
            Button b = sender as Button;
            QuantitativeRelationshipNode q = b.DataContext as QuantitativeRelationshipNode;
            MenuFlyout menu = new MenuFlyout();
            int index = (int)q.Type;
            string[] r = q.SubtypeNames[index];
            for (int i = 0; i < r.Length; i++)
            {
                MenuFlyoutItem item = new MenuFlyoutItem()
                {
                    Text = r[i],
                    Tag = i
                };
                item.Click += delegate (object s, RoutedEventArgs args)
                {
                    int inp = (int)((MenuFlyoutItem)s).Tag;
                    q.SubtypeSource = inp;
                    Exp.Save();
                };
                menu.Items.Add(item);
            }
            menu.ShowAt(b);
        }

        public async void Material_Image_Click (object sender, RoutedEventArgs e)
        {
            Button b = sender as Button;
            var picker = new Windows.Storage.Pickers.FileOpenPicker();
            picker.ViewMode = Windows.Storage.Pickers.PickerViewMode.Thumbnail;
            picker.SuggestedStartLocation =
                Windows.Storage.Pickers.PickerLocationId.PicturesLibrary;
            picker.FileTypeFilter.Add(".jpg");
            picker.FileTypeFilter.Add(".jpeg");
            picker.FileTypeFilter.Add(".png");
            picker.FileTypeFilter.Add(".bmp");

            Windows.Storage.StorageFile file = await picker.PickSingleFileAsync();
            if (file != null)
            {
                await ((MaterialNode)b.DataContext).AddImage(file);
                Exp.Save();
            }
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string output = "";
            foreach (char c in ((TextBox)sender).Text)
            {
                if (c >= '0' && c <= '9')
                {
                    output = output + c;
                }
                    
            }
            ((TextBox)sender).Text = output;
        }
    }
}