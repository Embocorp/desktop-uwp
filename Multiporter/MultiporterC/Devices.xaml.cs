using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace MultiporterC
{
    /// <summary>
    /// Page listing all connected devices
    /// </summary>
    public sealed partial class Devices : Page
    {
        private Device SelectedDevice;

        public Devices()
        {
            this.InitializeComponent();
            NavPanel.ExpandClick += new RoutedEventHandler(this.NavControl_Expand);
        }

        public async void PageLoaded(object sender, RoutedEventArgs e)
        {
            Progressing.Visibility = Visibility.Visible;
            var oldDevices = ConnectedDevices();
            var newDevices = FindDevices();
            await Task.WhenAll(oldDevices, newDevices);
            List<Device> connected = await oldDevices as List<Device>;
            List<Device> found = await newDevices as List<Device>;
            if (connected.Count == 0)
            {
                NoConnected.Visibility = Visibility.Visible;
            }
            else
            {
                NoConnected.Visibility = Visibility.Collapsed;
            }
            Progressing.Visibility = Visibility.Collapsed;
            for (int i = 0; i < connected.Count; i++)
            {
                connectedDevices.Items.Add(connected[i]);
            }
            for(int i = 0; i < found.Count; i++)
            {
                foundDevices.Items.Add(found[i]);
            }
        }

        private async Task<List<Device>> ConnectedDevices ()
        {
            return await CacheManagement.GetConnectedDevices();
        }

        private async Task<List<Device>> FindDevices()
        {
            List<Device> output = new List<Device>();
            // Production, this would find the device
            await Task.Delay(1000);

            // TODO: Remove this in production
            Liscence individual = new Liscence()
            {
                Key = "abcd1234",
                Name = "Theodore Kim",
                Type = Liscence.LiscenceType.Individual
            };
            Device d = new Device("Theo's Multicorder")
            {
                Sensors = getDeviceSensor(),
                Liscence = individual,
                Version = "Multicorder v0.1"
            };
            output.Add(d);
            // Remove this from production

            return output;
        }

        private void NavControl_Expand(object sender, RoutedEventArgs e)
        {
            MySplitView.IsPaneOpen = !MySplitView.IsPaneOpen;
        }

        private void NavPanel_Loaded(object sender, RoutedEventArgs e)
        {

        }

        async Task Progress(ProgressBar p)
        {
            await Task.Run(async () =>
            {
                for (int i = 0; i < 100; i++)
                {
                    if (i > 30 && i < 60)
                    {
                        await Task.Delay(200);
                    } 
                    else if (i < 30)
                    {
                        await Task.Delay(100);
                    }
                    else
                    {
                        await Task.Delay(100 - i);
                    }
                    await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                    {
                        p.Value++;
                    });
                }
            });
        }

        private async void FoundDevices_ItemClick(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                ProgressBar bar = new ProgressBar();
                Device sel = (Device)e.AddedItems[0];

                TextBlock nameTitle = new TextBlock();
                Sensor[] sensors;
                SolidColorBrush greyed = new SolidColorBrush(Color.FromArgb(0xFF, 0xAF, 0xAF, 0xAF));
                SolidColorBrush black = new SolidColorBrush(Color.FromArgb(0xFF, 0x00, 0x00, 0x00));

                Placeholder.Visibility = Visibility.Collapsed;

                bar.Width = 200;
                bar.Height = 10;
                bar.Maximum = 100;
                bar.Value = 0;
                bar.Foreground = new SolidColorBrush(Color.FromArgb(0xFF, 0x00, 0x71, 0xC1));


                //Add progress bar, wait for completion then remove the progress bar
                deviceInfo.Children.Add(bar);
                await Progress(bar);
                deviceInfo.Children.Remove(bar);

                //Load device information to the device info panel
                deviceInfoForm.Visibility = Visibility.Visible;
                deviceType.Text = sel.Version;
                deviceName.Text = (string)sel.Name;
                numberSensor.Text = sel.Sensors.Length.ToString() + " Sensors";
                deviceOwner.Text = sel.Liscence.Name;
                deviceLicense.Text = sel.Liscence.GetLiscenceType();
                sensors = sel.Sensors;
                sensorList.Items.Clear();
                for (int i = 0; i < sensors.Length; i++)
                {
                    sensorList.Items.Add(sensors[i]);
                }
                SelectedDevice = sel;
                ConnectButton.IsEnabled = !sel.Connected;
                if (sel.Connected)
                {
                    ConnectButton.Content = "Connected";
                }
            }
            else
            {
                Placeholder.Visibility = Visibility.Visible;
                deviceInfoForm.Visibility = Visibility.Collapsed;
                ConnectButton.Content = "Connect";
                ConnectButton.IsEnabled = true;
            }
        }

        private Sensor[] getDeviceSensor()
        {
            return new Sensor[]
            {
                new Sensor("Integrated Environmental Sensor", "BME680",
                    "Bosche Sensortec", new Measurement[] {
                            new Measurement("Relative Humidity", new string[] {"%"}),
                            new Measurement("Temperature", new string[] { "\u00b0C", "\u00b0F", "K" }),
                            new Measurement("Pressure", new string[] { "atm", "Pa" }),
                            new Measurement("Altitude", new string[] { "ft", "m" })
                        }),
                new Sensor("Integrated Motion Sensor", "BMX055",
                    "Bosche Sensortec", new Measurement[] {
                            new Measurement("Acceleration", new string[] { "m/s.s", "ft/s.s" }),
                            new Measurement("Orientation", new string[] { "\u00b0" })
                        }),
                new Sensor("LIDAR Time of Flight (ToF) Motion Sensor", "VL53L0X",
                    "ST MicroElectronics", new Measurement[] {
                            new Measurement("Acceleration", new string[] { "m/s.s", "ft/s.s" }),
                            new Measurement("Linear Distance", new string[] { "ft", "in", "cm", "m" }),
                            new Measurement("Velocity", new string[] { "m/s", "ft/s" })
                        }),
                new Sensor("Contactless Thermopile Sensor", "TMP006",
                    "Texas Instruments", new Measurement[] {
                            new Measurement("Temperature", new string[] { "\u00b0C", "\u00b0F", "K" }),
                        }),
                new Sensor("Integrated Air Quality Gas Sensor", "MiCS-5524",
                    "SGX Sensortek", new Measurement[] {
                        new Measurement("CO / Hydrocarbon Concentration", new string[] { "ppm" })
                    }),
                new Sensor("Hall Effect Sensor", "ACS722",
                    "Allegro Microsystems", new Measurement[] {
                        new Measurement("Current", new string[] { "A", "mA" }),
                        new Measurement("Resistance", new string[] { "\u2126" }),
                        new Measurement("Potential Difference", new string[] { "V" })
                    }),
                new Sensor("Three Spectrum Light Sensor", "SI1145",
                    "SiLabs", new Measurement[] {
                        new Measurement("UV Index", new string[] { "units" }),
                        new Measurement("IR Reflectance", new string[] { "%" }),
                        new Measurement("IR Absorbance", new string[] { "%" }),
                        new Measurement("IR Transmittance", new string[] { "%" }),
                        new Measurement("Visible Flux", new string[] {"lumens" })
                    })
            };
        }

        private async void SensorInformation(object sender, SelectionChangedEventArgs e)
        {
            Sensor s = ((Sensor)e.AddedItems[0]);
            ContentDialog SensorDialog = new ContentDialog()
            {
                Title = s.Name,
                Content = "Manufacturer: "+ s.Manufacturer + "\nPart Name: " + s.Part + "\nSupported Measurements: " + s.GetMeasurements(),
                PrimaryButtonText = "Close"
            };

            await SensorDialog.ShowAsync();
        }

        public async Task Connect()
        {
            CacheManagement.ConnectDevice(SelectedDevice);
            await Task.Delay(2000);
        }
        
        public async void ConnectDevice(object sender, RoutedEventArgs e)
        {
            SolidColorBrush transparent = new SolidColorBrush(Color.FromArgb(0x00, 0xAF, 0xAF, 0xAF));
            Button butt = (Button)sender;
            ProgressBar bar = new ProgressBar()
            {
                IsIndeterminate = true,
                Width = 100,
                Height = 25
            };
            butt.Content = bar;
            await Connect();
            ((ListViewItem)foundDevices.ContainerFromItem(SelectedDevice)).IsSelected = false;
            foundDevices.Items.Remove(SelectedDevice);
            connectedDevices.Items.Add(SelectedDevice);
            butt.Content = "Connected";
            butt.Background = transparent;
            butt.IsEnabled = false;
            SelectedDevice.Connected = true;
            NoConnected.Visibility = Visibility.Collapsed;
        }
    }
}
