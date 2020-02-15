using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace WDHAN
{
    public class Page
    {
        public JObject frontmatter { get; set; }
        public string content { get; set; }
        public string url { get; set; }
        public Page()
        {
            
        }
        public static string getPageContents(string filePath)
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
                    return JObject.Parse("{\"null\": true}");
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
                return JObject.Parse("{\"null\": true}");
            }
        }
    }
}