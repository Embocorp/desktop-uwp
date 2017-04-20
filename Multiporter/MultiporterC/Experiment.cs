using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

namespace MultiporterC
{
    public class Experiment:BaseClass
    {

        public Experiment(String name, String created, String author)
        {
            Name = name;
            Created = created;
            Author = author;
            Cards = new List<ExperimentNode>();
        }

        public Experiment() { }

        public void AddNode (ExperimentNode newNode)
        {
            Cards.Add(newNode);
        }

        public void RemoveNode (ExperimentNode deleteNode)
        {
            Cards.Remove(deleteNode);
        }

        public async void Save()
        {
            await Task.Run(async () =>
            {
                // Create sample file; replace if exists.
                Windows.Storage.StorageFolder storageFolder =
                    Windows.Storage.ApplicationData.Current.LocalFolder;
                Windows.Storage.StorageFile sampleFile =
                    await storageFolder.CreateFileAsync(FileName + ".mport",
                    Windows.Storage.CreationCollisionOption.ReplaceExisting);
                string serial = Serialize.Serialize_Object<Experiment>(this);
                try
                {
                    await Windows.Storage.FileIO.WriteBytesAsync(sampleFile, Encoding.UTF8.GetBytes(serial));
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.Message);
                }
            });
        }

        public async Task<BitmapImage> Get_Thumbnail()
        {
            if (Thumbnail == null || Thumbnail == "")
            {
                throw new Exception("No Thumbnail Assigned");
            }
            byte[] array = Convert.FromBase64String(Thumbnail);
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
            }
        }

        public async Task Save_Thumbnail(StorageFile f)
        {
            Stream x = await f.OpenStreamForReadAsync();
            byte[] buffer = new byte[16*1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = x.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                Thumbnail = Convert.ToBase64String(ms.ToArray());
                Debug.WriteLine(Thumbnail);
            }
        }

        public List<VariableNode> GetVariables(VariableNode.VariableType type)
        {
            List<VariableNode> output = new List<VariableNode>();
            for (int i = 0; i < Cards.Count; i++)
            {
                if (Cards[i].GetType() == typeof(VariableNode) && (Cards[i] as VariableNode).Type == type)
                {
                    output.Add(Cards[i] as VariableNode);
                }
            }
            return output;
        }

        public List<ExperimentNode> Cards { get; set; }
        public string Name { get; set; }
        public string Created { get; set; }
        public string Author { get; set; }
        public string FileName { get; set; }
        public string Thumbnail { get; set; }
        public ExperimentVisibility Sharing { get; set; }
        public User[] Contributors { get; set; }
        public Device[] Devices { get; set; }

        [XmlIgnoreAttribute]
        public BitmapImage BMPThumb { get; set; }
        [XmlIgnoreAttribute]
        public string Thumnbail_Handler { set { Thumbnail = value; OnPropertyChanged("BMPThumb"); } }

        public enum ExperimentVisibility { Visible, Private };
    }
}
