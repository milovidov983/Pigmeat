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
        public string path { get; set; }

        // The above three variables should be specified. The rest are defined by the Constructor.
        public string url { get; set; }
        public DateTime date { get; set; }
        public List<string> tags { get; set; }
        public string dir { get; set; }
        public string name { get; set; }
        public string excerpt { get; set; }
        public Page()
        {
            getDefinedPage(this);
        }
        public static Page getDefinedPage(Page page)
        {
            page.name = Path.GetFileName(page.path);
            page.dir = Path.GetDirectoryName(page.path);

            try
            {
                page.tags = JsonConvert.DeserializeObject<List<string>>(page.frontmatter.GetValue("tags").ToString());
            }
            catch(NullReferenceException)
            {

            }

            try
            {
                page.date = JsonConvert.DeserializeObject<DateTime>(page.frontmatter.GetValue("date").ToString());
            }
            catch(NullReferenceException)
            {

            }

            try
            {
                page.excerpt = getExcerpt(page);
            }
            catch(NullReferenceException)
            {

            }

            return page;
        }
        public static string getExcerpt(Page page)
        {
            string excerpt = "";
            foreach(var currentChar in page.content)
            {
                excerpt += currentChar;
                try
                {
                    if(excerpt.Contains(page.frontmatter["excerpt_separator"].ToString()))
                    {
                        excerpt = excerpt.Substring(0, page.frontmatter["excerpt_separator"].ToString().Length);
                        break;
                    }
                }
                catch(NullReferenceException)
                {
                    if(excerpt.Contains(GlobalConfiguration.getConfiguration().excerpt_separator))
                    {
                        excerpt = excerpt.Substring(0, GlobalConfiguration.getConfiguration().excerpt_separator.Length);
                        break;
                    }
                }
            }
            return excerpt;
        }
        public static JObject getPage(Page page)
        {
            return JObject.Parse(JsonConvert.SerializeObject(page));
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
    }
}