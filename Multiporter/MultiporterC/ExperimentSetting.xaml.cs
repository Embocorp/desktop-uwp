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
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace MultiporterC
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ExperimentSetting : Page
    {
        private Experiment Exp;

        public ExperimentSetting()
        {
            this.InitializeComponent();
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            Experiment exp = e.Parameter as Experiment;
            Exp = exp;
            ExpName.Text = exp.Name;
            ExperimentCreated.Text = exp.Created;
            ExperimentAuthor.Text = exp.Author;
            if (Exp.Thumbnail != null && Exp.Thumbnail != "")
            {
                Thumb.Source = await Exp.Get_Thumbnail();
            }
        }

        public async void New_Thumb (object sender, RoutedEventArgs e)
        {
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
                await Exp.Save_Thumbnail(file);
                Thumb.Source = await Exp.Get_Thumbnail();
            }
        }

        public void Save_Click(object sender, RoutedEventArgs e)
        {
            if (ExpName.Text != "")
            {
                Exp.Name = ExpName.Text;
            }
            Exp.Save();
            this.Frame.Navigate(typeof(Explorer), Exp);
        }
    }
}
