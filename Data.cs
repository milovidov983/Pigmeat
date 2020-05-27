using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json;

namespace Pigmeat
{
    public class Data
    {
        public Dictionary<string, object> data { get; set; }
        public Data()
        {

        }
        public static Dictionary<string, object> getGlobalData()
        {
            try
            {
                var siteConfig = GlobalConfiguration.getConfiguration();
                Dictionary<string, object> dataContents = new Dictionary<string, object>();
                if(Directory.GetFiles(siteConfig.source + "/" + siteConfig.data_dir).Length == 0)
                {
                    return new Dictionary<string, object>() { { "data", null } };
                }
                else
                {
                    //dataContents = JObject.Parse("{\"hasData\": true}");
                    foreach(var file in Directory.GetFiles(siteConfig.source + "/" + siteConfig.data_dir))
                    {
                        dataContents.Add(Path.GetFileNameWithoutExtension(file), GlobalConfiguration.yamlInterop(File.ReadAllText(file).ToString()));
                    }
                }
                return dataContents;
            }
            catch(Newtonsoft.Json.JsonReaderException ex)
            {
                Console.WriteLine(ex);
                Console.WriteLine("ERROR [Newtonsoft.Json.JsonReaderException]: Failed to read global project data. Ensure every file is in a supported data format, and has content.");
                return new Dictionary<string, object>() { { "data", null } };
            }
        }
        public static void generateDataIndex()
        {
            var siteConfig = GlobalConfiguration.getConfiguration();
            Directory.CreateDirectory(siteConfig.source + "/" + siteConfig.data_dir);
            Data dataSet = new Data();
            dataSet.data = getGlobalData();
            string dataSerialized = JsonConvert.SerializeObject(dataSet, Formatting.Indented);
            Directory.CreateDirectory(siteConfig.source + "/temp");
            using (FileStream fs = File.Create(siteConfig.source + "/temp/_data.json"))
            {
                fs.Write(Encoding.UTF8.GetBytes(dataSerialized), 0, Encoding.UTF8.GetBytes(dataSerialized).Length);
            }
        }
    }
}
