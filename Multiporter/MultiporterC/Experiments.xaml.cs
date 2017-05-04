using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace MultiporterC
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Experiments : Page
    {
        public Experiments()
        {
            this.InitializeComponent();
            NavPanel.ExpandClick += new RoutedEventHandler(this.NavControl_Expand);
        }

        public async void PageLoaded(object sender, RoutedEventArgs e)
        {
            Experiment[] exp, local;
            OnlineLoading.Visibility = Visibility.Visible;
            LocalLoading.Visibility = Visibility.Visible;
            local = await LoadExperiments();
            for (int i = 0; i < local.Length; i++)
            {
                if (local[i].Thumbnail == null || local[i].Thumbnail == "")
                {
                    BitmapImage bitmapImage = new BitmapImage(new Uri("ms-appx:/Assets/placeholder.png"));
                    local[i].BMPThumb = bitmapImage;
                }
                else
                {
                    local[i].BMPThumb = await local[i].Get_Thumbnail();
                }
                ExperimentsList.Items.Add(local[i]);
            }
            LocalLoading.Visibility = Visibility.Collapsed;
            exp = await LoadOnlineExperiments();
            for (int i = 0; i < exp.Length; i++)
            {
                OnlineExperimentsList.Items.Add(exp[i]);
            }
            OnlineLoading.Visibility = Visibility.Collapsed;
            if ( exp.Length == 0 && Windows.Storage.ApplicationData.Current.LocalSettings.Values["usertoken"] == null)
            {
                Rex.Text = "You must be logged on to access this feature";
            }
        }

        public void OpenExperiment(object sender, SelectionChangedEventArgs e)
        {
            this.Frame.Navigate(typeof(Explorer), e.AddedItems[0]);
        }

        private void NavControl_Expand(object sender, RoutedEventArgs e)
        {
            MySplitView.IsPaneOpen = !MySplitView.IsPaneOpen;
        }

        private async Task<Experiment[]> LoadExperiments()
        {
            List<Experiment> output = new List<Experiment>();
            Windows.Storage.StorageFolder storageFolder =
                    Windows.Storage.ApplicationData.Current.LocalFolder;
            var files = await storageFolder.GetFilesAsync();
            foreach (StorageFile f in files) {
                string xml = await Windows.Storage.FileIO.ReadTextAsync(f);
                try
                {
                    output.Add((Experiment)Serialize.Xml_Deserialize_Object<Experiment>(xml));
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.Message);
                }
            }
            return output.ToArray();
        }

        private async Task<Experiment[]> LoadOnlineExperiments()
        {
            if (Windows.Storage.ApplicationData.Current.LocalSettings.Values["usertoken"] == null)
            {
                return new Experiment[0];
            }
            else
            {
                await Task.Delay(2000);
                return new Experiment[]
                {
                new Experiment("Christmas Tree Lights", "December 25, 2016", "Megan Brown"),
                new Experiment("Humidity vs Barometric Pressure", "February 1, 2016", "Zainab Babikir")
                };
            }
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        // Handles the Click event on the Button inside the Popup control and 
        // closes the Popup. 
        private void ClosePopupClicked(object sender, RoutedEventArgs e)
        {
            // if the Popup is open, then close it 
            if (StandardPopup.IsOpen) { StandardPopup.IsOpen = false; }
        }

        private void New_Experiment_Click(object sender, RoutedEventArgs e)
        {
            if (!StandardPopup.IsOpen) { StandardPopup.IsOpen = true; }
        }

        private void Create_Experiment_Click(object sender, RoutedEventArgs e)
        {
            object auth = Windows.Storage.ApplicationData.Current.LocalSettings.Values["username"];
            string author;
            if (auth == null)
            {
                author = "Anonymous";
            }
            else
            {
                author = (string)auth;
            }
            Experiment newExp = new Experiment(ExperimentName.Text, 
                DateTime.Today.ToString("MMMM dd, yyyy"), 
                author);
            newExp.FileName = DateTime.Now.ToString("yyyyMMddHHmmss");
            newExp.Create();
            this.Frame.Navigate(typeof(Explorer), newExp);
        }
    }
}
