using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using Windows.Data.Xml.Dom;
using System.Net;
using System.IO;
using System.Diagnostics;
using System.Xml;
using Newtonsoft.Json;

namespace MultiporterC
{
    public static class HttpHelpers
    {
        private static readonly HttpClient client = new HttpClient();
        public enum DocType { Json, Xml };

        public static string XmlToJSON(string xml)
        {
            System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
            doc.LoadXml(xml);
            string json = Newtonsoft.Json.JsonConvert.SerializeXmlNode(doc);
            return json;
        }

        public async static Task<T> Post<T>(object data, string api)
        {
            string json = Serialize.Json_Serialize_Object<T>((T)data);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://localhost:8081/api/" + api);
            request.ContentType = "application/json; encoding='utf-8'";
            request.Method = "POST";

            using (var streamWriter = new StreamWriter(await request.GetRequestStreamAsync()))
            {
                streamWriter.Write(json);
                streamWriter.Flush();
            }
            
            HttpWebResponse response;
            try
            {
                response = (HttpWebResponse)await request.GetResponseAsync();
            }
            catch (WebException e)
            {
                response = e.Response as HttpWebResponse;
            }
            using (var reader = new System.IO.StreamReader(response.GetResponseStream()))
            {
                string responseText = reader.ReadToEnd();
                try
                {
                    return JsonConvert.DeserializeObject<T>(responseText);
                }
                catch (JsonReaderException ex)
                {
                    Debug.WriteLine(ex.Message);
                    return default(T);
                }
            }
        }

        public async static Task<T> Put<T>(object data, string api)
        {
            string json = Serialize.Json_Serialize_Object<T>((T)data);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://localhost:8081/api/" + api);
            request.ContentType = "application/json; encoding='utf-8'";
            request.Method = "PUT";

            using (var streamWriter = new StreamWriter(await request.GetRequestStreamAsync()))
            {
                streamWriter.Write(json);
                streamWriter.Flush();
            }

            HttpWebResponse response;
            try
            {
                response = (HttpWebResponse)await request.GetResponseAsync();
            }
            catch (WebException e)
            {
                response = e.Response as HttpWebResponse;
            }
            using (var reader = new System.IO.StreamReader(response.GetResponseStream()))
            {
                string responseText = reader.ReadToEnd();
                try
                {
                    return JsonConvert.DeserializeObject<T>(responseText);
                }
                catch (JsonReaderException ex)
                {
                    Debug.WriteLine(ex.Message);
                    return default(T);
                }
            }
        }

        public async static Task<T> Get<T>(Dictionary<string, string> data, string api)
        {
            var result = new List<string>();
            foreach (KeyValuePair<string, string> entry in data)
            {
                result.Add(entry.Key + "=" + entry.Value);
            }

            string query = string.Join("&", result);

            string uri = "http://localhost:8081/api/" + api + "?" + query;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
            request.Method = "GET";

            HttpWebResponse response;
            try
            {
                response = (HttpWebResponse)await request.GetResponseAsync();
            }
            catch (WebException e)
            {
                response = e.Response as HttpWebResponse;
            }
            using (var reader = new System.IO.StreamReader(response.GetResponseStream()))
            {
                string responseText = reader.ReadToEnd();
                try
                {
                    return JsonConvert.DeserializeObject<T>(responseText);
                }
                catch (JsonReaderException ex)
                {
                    Debug.WriteLine(ex.Message);
                    return default(T);
                }
            }
        }
    }
}
