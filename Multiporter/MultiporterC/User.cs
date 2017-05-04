using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiporterC
{
    public class User : BaseClass
    {
        public User() { }

        public async void Save()
        {
            string serial = JsonConvert.SerializeObject(this);
            Windows.Storage.StorageFolder storageFolder =
                    Windows.Storage.ApplicationData.Current.LocalCacheFolder;
            Windows.Storage.StorageFile sampleFile =
                await storageFolder.CreateFileAsync("user.txt",
                Windows.Storage.CreationCollisionOption.ReplaceExisting);
            try
            {
                await Windows.Storage.FileIO.WriteBytesAsync(sampleFile, Encoding.UTF8.GetBytes(serial));
}
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
        }

        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string First { get; set; }
        public string Last { get; set; }
        public string Type { get; set; }
        public int Experiments { get; set; }
        public string Avatar { get; set; }
        public int Points { get; set; }
    }
}
