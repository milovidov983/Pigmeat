using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace WDHAN
{
    public class WDHANFile
    {
        public JObject frontmatter { get; set; }
        public string content { get; set; }
        public string path { get; set; }
        public DateTime modified_time { get; set; }
        public string name { get; set; }
        public string basename { get; set; }
        public string extname { get; set; }
        public string title { get; set; }
        public WDHANFile()
        {
            
        }
        public static string getFileContents(string filePath)
        {
            var siteConfig = GlobalConfiguration.getConfiguration();
            string fileContents = "";
            Boolean first = false;
            Boolean second = false;
            if(File.ReadAllLines(filePath)[0].Equals("---", StringComparison.OrdinalIgnoreCase))
            {
                foreach(var line in File.ReadAllLines(filePath)){
                    if(line.Equals("---", StringComparison.OrdinalIgnoreCase) && !first)
                    {
                        first = true;
                        continue;
                    }
                    if(line.Equals("---", StringComparison.OrdinalIgnoreCase) && first)
                    {
                        second = true;
                        continue;
                    }
                    if(first && second)
                    {
                        fileContents += (line + "\n");
                    }
                }
            }
            else
            {
                fileContents = File.ReadAllText(filePath);
            }
            return fileContents;
        }
        public static JObject parseFrontMatter(string filePath)
        {
            Boolean first = false, second = false;
            string pageFrontMatter = "";
            if(File.ReadAllLines(filePath)[0].Equals("---", StringComparison.OrdinalIgnoreCase))
            {
                foreach(var line in File.ReadAllLines(filePath)){
                    if(line.Equals("---", StringComparison.OrdinalIgnoreCase) && !first)
                    {
                        first = true;
                        continue;
                    }
                    if(line.Equals("---", StringComparison.OrdinalIgnoreCase) && first)
                    {
                        second = true;
                        continue;
                    }
                    if(first && !second)
                    {
                        pageFrontMatter += (line + "\n");
                    }
                }
                if(pageFrontMatter.Equals("", StringComparison.OrdinalIgnoreCase))
                {
                    return JObject.Parse("{\"exists\": true}");
                }
                else
                {
                    Console.WriteLine("Frontmatter:\n" + pageFrontMatter);
                    return GlobalConfiguration.yamlInterop(pageFrontMatter);
                    /*
                    if(isJSON(pageFrontMatter))
                    {
                        return JObject.Parse(pageFrontMatter);
                    }
                    else
                    {
                        var deserializer = new Deserializer();
                        StringReader reader = new StringReader(pageFrontMatter);
                        var yamlObject = deserializer.Deserialize(reader);
                        return JObject.Parse(JsonConvert.SerializeObject(yamlObject, Formatting.Indented));
                    }
                    */
                }
            }
            else
            {
                return JObject.Parse("{\"exists\": false}");
            }
        }
        public static WDHANFile getDefinedFile(WDHANFile file)
        {
            file.modified_time = System.IO.File.GetLastWriteTimeUtc("./" + file.path);
            file.basename = Path.GetFileNameWithoutExtension("./" + file.path);
            file.extname = Path.GetExtension("./" + file.path);
            return file;
        }
        public string getTitle()
        {
            try
            {
                return frontmatter["title"].ToString();
            }
            catch(NullReferenceException)
            {
                return "";
            }
        }
    }
}