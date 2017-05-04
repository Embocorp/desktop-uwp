using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace MultiporterC
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class LoginPage : Page
    {
        private string _username = "";
        private string _password = "";

        public string Username {
            get
            {
                return _username;
            }
            set
            {
                _username = value ;
            }
        }
        public string Password
        {
            get
            {
                return _password;
            }
            set
            {
                _password = value;
            }
        }

        public LoginPage()
        {
            this.InitializeComponent();
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            Button b = sender as Button;
            ProgressRing ring = new ProgressRing()
            {
                Width = 25,
                Height = 25,
                Foreground = new SolidColorBrush(Colors.Black),
                IsActive = true
            };
            b.Content = ring;
            Dictionary<String, String> dict = new Dictionary<string, string>();
            Dictionary<String, object> response = new Dictionary<string, object>();
            Dictionary<String, object> user = new Dictionary<string, object>();
            string dialogText = "";
            var settings = ApplicationData.Current.LocalSettings;
            ContentDialog d = new ContentDialog()
            {
                PrimaryButtonText = "Ok"
            };

            if (_username == "" && _password == "")
            {
                dialogText = "Please fill out all of the required fields";
            }
            else
            {
                dict.Add("Username", _username);
                dict.Add("Password", _password);

                response = await HttpHelpers.Post<Dictionary<string, object>>(dict, "auth/login");
                if (response == null)
                {
                    dialogText = "Sorry, something went wrong";
                }
                else if (response.ContainsKey("success") && (bool)response["success"] == true)
                {
                    dialogText = "Welcome, " + Username + "!";
                    d.PrimaryButtonClick += delegate
                    {
                        this.Frame.Navigate(typeof(UserPage));
                    };
                    
                    user = await HttpHelpers.Get<Dictionary<string, object>>(dict, "user/username/" + Username);
                    if ((bool)user["success"] == true)
                    {
                        User userdata = ((JObject)user["data"]).ToObject<User>();
                        userdata.Save();
                        settings.Values["usertoken"] = response["token"];
                        settings.Values["username"] = Username;
                    }
                    else
                    {
                        dialogText = "What??";
                    }
                }
                else
                {
                    Dictionary<string, object> error = ((JObject)response["error"]).ToObject<Dictionary<string, object>>();
                    if ((string)error["type"] == "Password")
                    {
                        dialogText = "Incorrect Password";
                    }
                    else if ((string)error["type"] == "Database")
                    {
                        dialogText = "Please include all of the required information";
                    }
                    else if ((string)error["type"] == "NoUser")
                    {
                        dialogText = "Sorry, that user doesn't exist.";
                    }
                    else
                    {
                        dialogText = "Oops, something went wrong";
                    }
                }
            }
            d.Title = dialogText;

            await d.ShowAsync();
        }
    }
}
