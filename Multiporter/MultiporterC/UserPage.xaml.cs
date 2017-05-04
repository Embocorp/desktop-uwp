using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage.Streams;
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
    public class Change
    {
        public Change() { }
        public Change(DateTime timestamp, string experiment, BitmapImage image, Dictionary<string, string> revisions)
        {
            if (revisions["type"] == "update")
            {
                if (revisions["property"] == "Thumbnail")
                {
                    Message = "You updated the Experiment Thumbnail";
                }
                else
                {
                    Message = "You " + revisions["type"] + "d the " + revisions["property"] + " from \"" + revisions["old"] + "\" to \"" + revisions["new"] + "\"";
                }
            }
            else
            {
                if (revisions["property"] == "Experiment")
                {
                    Message = "You started a new experiment named " + revisions["new"];
                }
                
                else
                {
                    Message = "You " + revisions["type"] + "d a new " + revisions["property"] + " card.";
                }
            }
            Experiment = experiment;
            Created = timestamp;
            Time = timestamp.ToString();
            Pic = image;
        }
        
        public BitmapImage Pic{ get; set; }
        public string Experiment { get; set; }
        public DateTime Created { get; set; }
        public string Time { get; set; }
        public string Message { get; set; }
    }
    public sealed partial class UserPage : Page, INotifyPropertyChanged
    {
        private User active;
        private string _username;
        private string _firstname;
        private string _lastname;
        private string _email;
        private int _exp;
        private int _points;

        private int limit = 5;
        private int offset = 0;

        public event PropertyChangedEventHandler PropertyChanged;

        public UserPage()
        {
            this.InitializeComponent();
        }

        private async Task<User> GetUser()
        {
            Dictionary<string, string> data = new Dictionary<string, string>();
            string uri = "user/username/" + Windows.Storage.ApplicationData.Current.LocalSettings.Values["username"];
            Dictionary<string, object> feedraw = await HttpHelpers.Get<Dictionary<string, object>>(data, uri);
            User second = ((JObject)feedraw["data"]).ToObject<User>();
            return second;
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            var settings = Windows.Storage.ApplicationData.Current.LocalSettings;
            if (settings.Values["usertoken"] == null || (string)settings.Values["usertoken"] == "")
            {
                this.Frame.Navigate(typeof(LoginPage));
            }
            else
            {
                active = await GetUser();
                _firstname = active.First;
                _lastname = active.Last;
                _username = active.Username;
                _email = active.Email;
                _exp = active.Experiments;
                _points = active.Points;
                
                OnPropertyChanged("Fullname");
                OnPropertyChanged("Username");
                OnPropertyChanged("Email");
                OnPropertyChanged("Points");
                OnPropertyChanged("Experiments");

                offset = 0;

                LoadFeed();
            }
        }

        private async Task<BitmapImage> GenImage (string input)
        {
            byte[] array = Convert.FromBase64String(input);
            using (InMemoryRandomAccessStream stream = new InMemoryRandomAccessStream())
            {
                using (DataWriter writer = new DataWriter(stream.GetOutputStreamAt(0)))
                {
                    writer.WriteBytes(array);
                    await writer.StoreAsync();
                }
                BitmapImage image = new BitmapImage();
                await image.SetSourceAsync(stream);
                return image;
                //Debug.WriteLine(temp);
            }
        }

        public async void LoadFeed ()
        {
            
            List<Change> output = new List<Change>();
            Dictionary<string, string> data = new Dictionary<string, string>();
            data.Add("limit", limit.ToString());
            data.Add("offset", offset.ToString());
            string uri = "user/feed/" + Windows.Storage.ApplicationData.Current.LocalSettings.Values["username"];
            Dictionary<string, object> feedraw = await HttpHelpers.Get<Dictionary<string, object>>(data, uri);
            List< Dictionary < string, object>> experiments = ((JArray)feedraw["data"]).ToObject<List<Dictionary<string, object>>>();
            foreach (Dictionary<string, object> ind in experiments)
            {
                BitmapImage temp = new BitmapImage(new Uri("ms-appx:///Assets/placeholder.png")); ;
                if (ind.ContainsKey("meta"))
                {
                    if (ind["meta"] != null)
                    {
                        Debug.WriteLine("Test");
                        temp = await GenImage(ind["meta"] as string);
                    }
                }
                foreach (string key in ind.Keys)
                {
                    if (key != "meta")
                    {
                        List<object> l = ((JArray)ind[key]).ToObject<List<object>>();
                        for (int i = 0; i < l.Count; i++)
                        {
                            Dictionary<string, object> top = ((JObject)l[i]).ToObject<Dictionary<string, object>>();
                            Dictionary<string, object> middle = ((JObject)top["changes"]).ToObject<Dictionary<string, object>>();
                            foreach (string keytwo in middle.Keys)
                            {
                                Dictionary<string, string> bottom = ((JObject)middle[keytwo]).ToObject<Dictionary<string, string>>();
                                output.Add(new Change((DateTime)top["timestamp"], key, temp, bottom));
                            }
                        }
                    }
                }
            }
            output.Sort((x, y) => DateTime.Compare(y.Created, x.Created));
            for (int i = 0; i < output.Count; i++)
            {
                Feed.Items.Add(output[i]);
            }
            Waiting.Visibility = Visibility.Collapsed;
        }

        private void OnScrollViewerViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            ScrollViewer sv = sender as ScrollViewer;
            var verticalOffset = sv.VerticalOffset;
            var maxVerticalOffset = sv.ScrollableHeight; //sv.ExtentHeight - sv.ViewportHeight;

            if (maxVerticalOffset < 0 ||
                verticalOffset == maxVerticalOffset)
            {
                // Scrolled to bottom
                offset += 10;
                LoadFeed();
            }
            else
            {
                // Not scrolled to bottom
                //Debug.WriteLine("Bottom")
            }
        }


        private void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public string Username
        {
            get
            {
                return _username;
            }
        }
        public string Fullname
        {
            get
            {
                return _firstname + " " + _lastname;
            }
        }

        public string Email
        {
            get
            {
                return _email;
            }
        }
        public string Experiments
        {
            get
            {
                return _exp.ToString();
            }
        }
        public string Points
        {
            get
            {
                return _points.ToString();
            }
        }
    }
}
