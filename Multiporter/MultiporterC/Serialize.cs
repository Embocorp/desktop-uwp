using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace MultiporterC
{
    class Serialize
    {
        public static string Serialize_Object<T> (T obj)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));

            using (StringWriter writer = new StringWriter())
            {
                serializer.Serialize(writer, obj);
                return writer.ToString();
            }
        }

        public static object Deserialize_Object<T> (string contents)
        {    
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            object result;

            using (StringReader reader = new StringReader(contents))
            {
                result = serializer.Deserialize(reader);
                return result;
            }
        }

        public static async void SaveExperiment(Experiment e)
        {
            await Task.Run(async () =>
            {
                // Create sample file; replace if exists.
                Windows.Storage.StorageFolder storageFolder =
                    Windows.Storage.ApplicationData.Current.LocalFolder;
                Windows.Storage.StorageFile sampleFile =
                    await storageFolder.CreateFileAsync(e.FileName + ".mport",
                    Windows.Storage.CreationCollisionOption.ReplaceExisting);
                string serial = Serialize.Serialize_Object<Experiment>(e);
                await Windows.Storage.FileIO.WriteBytesAsync(sampleFile, Encoding.UTF8.GetBytes(serial));
                Debug.WriteLine(storageFolder.Path);
            });
        }
    }

    class CacheManagement
    {
        public static async void ConnectDevice(Device d)
        {
            await Task.Run(async () =>
            {
                List<Device> output = new List<Device>();
                // Open File
                Windows.Storage.StorageFolder storageFolder =
                    Windows.Storage.ApplicationData.Current.LocalCacheFolder;
                Windows.Storage.StorageFile cacheFile;

                cacheFile = await storageFolder.CreateFileAsync("devices.xml", Windows.Storage.CreationCollisionOption.OpenIfExists);
                string xml = await Windows.Storage.FileIO.ReadTextAsync(cacheFile);
                if (xml != "")
                {
                    try
                    {
                        output = (List<Device>)Serialize.Deserialize_Object<List<Device>>(xml);
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine(e.Message);
                    }
                }

                output.Add(d);
                string serial = Serialize.Serialize_Object<List<Device>>(output);

                try
                {
                    await Windows.Storage.FileIO.WriteBytesAsync(cacheFile, Encoding.UTF8.GetBytes(serial));
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.Message);
                }
            });
        }

        public static async Task<List<Device>> GetConnectedDevices()
        {
            List<Device> output = new List<Device>();
            // Open File
            Windows.Storage.StorageFolder storageFolder =
                Windows.Storage.ApplicationData.Current.LocalCacheFolder;
            Windows.Storage.StorageFile cacheFile;

            cacheFile = await storageFolder.CreateFileAsync("devices.xml", Windows.Storage.CreationCollisionOption.OpenIfExists);
            string xml = await Windows.Storage.FileIO.ReadTextAsync(cacheFile);
            if (xml != "")
            {
                try
                {
                    output = (List<Device>)Serialize.Deserialize_Object<List<Device>>(xml);
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.Message);
                }
            }
            return output;
        }


        public static async void CheckLocalConnections (Boolean newSession)
        {
            List<Device> devices = await GetConnectedDevices();
            Windows.Storage.StorageFolder storageFolder =
                    Windows.Storage.ApplicationData.Current.LocalCacheFolder;
            Windows.Storage.StorageFile cacheFile = await storageFolder.CreateFileAsync("devices.xml", Windows.Storage.CreationCollisionOption.OpenIfExists);
            if (newSession)
            {
                await cacheFile.DeleteAsync();
            }
            else
            {
                await Task.Delay(1000);
            }
        }
    }
}
