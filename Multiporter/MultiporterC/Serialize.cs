using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace MultiporterC
{
    class Serialize
    {
        public static string Xml_Serialize_Object<T>(T obj)
        {
            XmlSerializer xsSubmit = new XmlSerializer(typeof(T));
            var xml = "";

            using (var sww = new StringWriter())
            {
                using (XmlWriter writer = XmlWriter.Create(sww))
                {
                    xsSubmit.Serialize(writer, obj);
                    xml = sww.ToString(); // Your XML
                    return xml;
                }
            }
        }

        public static object Xml_Deserialize_Object<T>(string contents)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            StringReader rdr = new StringReader(contents);
            object output = (object)serializer.Deserialize(rdr);
            return output;
        }

        public static string Json_Serialize_Object<T> (T obj)
        {
            
            return JsonConvert.SerializeObject(obj);
        }

        public static object Json_Deserialize_Object<T> (string contents)
        {
           
            return JsonConvert.DeserializeObject<T>(contents);
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
                string serial = Serialize.Xml_Serialize_Object<Experiment>(e);
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
                        output = (List<Device>)Serialize.Xml_Deserialize_Object<List<Device>>(xml);
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine(e.Message);
                    }
                }

                output.Add(d);
                string serial = Serialize.Xml_Serialize_Object<List<Device>>(output);

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
                    output = (List<Device>)Serialize.Xml_Deserialize_Object<List<Device>>(xml);
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
    public static class ByteCompression
    {
        public static byte[] Compress(byte[] data)
        {
            MemoryStream output = new MemoryStream();
            using (DeflateStream dstream = new DeflateStream(output, CompressionLevel.Optimal))
            {
                dstream.Write(data, 0, data.Length);
            }
            return output.ToArray();
        }

        public static byte[] Decompress(byte[] data)
        {
            MemoryStream input = new MemoryStream(data);
            MemoryStream output = new MemoryStream();
            using (DeflateStream dstream = new DeflateStream(input, CompressionMode.Decompress))
            {
                dstream.CopyTo(output);
            }
            return output.ToArray();
        }

    }
}
