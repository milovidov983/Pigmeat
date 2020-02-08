using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using YamlDotNet.Serialization;
using YamlDotNet;

namespace WDHAN
{
    public class Post
    {
        public JObject frontmatter { get; set; }
        public string content { get; set; }
        public Post()
        {
            
        }
        public static string getPostContents(string filePath)
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
        public static Boolean isMarkdown(string fileExt)
        {
            foreach(var ext in GlobalConfiguration.getMarkdownExts())
            {
                if(ext.Equals(fileExt, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }
            return false;
        }
        public static Dictionary<string, object> getPosts(string collectionName) 
        {
            var siteConfig = GlobalConfiguration.getConfiguration();
            Dictionary<string, object> postList = new Dictionary<string, object>();
            int i = 0;
            foreach(var collection in siteConfig.collections)
            {
                if(collection.Equals(collectionName, StringComparison.OrdinalIgnoreCase))
                {
                    foreach(var post in Directory.GetFiles(siteConfig.collections_dir + "/_" + collection))
                    {
                        if(isMarkdown(Path.GetExtension(post).Substring(1)))
                        {
                            //postList.Add(i.ToString(), getPostContents(post));
                            postList.Add(parseFrontMatter(post)["title"].ToString(), getPostContents(post));
                            i++;
                        }
                    }
                }
            }

            foreach(var post in postList)
            {
                Console.WriteLine(post);
            }

            return postList;
        }
        public static void generateEntires(string collectionName)
        {
            Collection collectionPosts = new Collection();
            collectionPosts.entries = getPosts(collectionName);
            string collectionSerialized = JsonConvert.SerializeObject(collectionPosts, Formatting.Indented);
            using (FileStream fs = File.Create("./_" + collectionName + "/_entries.json"))
            {
                fs.Write(Encoding.UTF8.GetBytes(collectionSerialized), 0, Encoding.UTF8.GetBytes(collectionSerialized).Length);
            }
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
                }
            }
            else
            {
                return JObject.Parse("{\"null\": true}");
            }
        }
        private static bool isJSON(string input)
        {
            input = input.Trim();
            if ((input.StartsWith("{") && input.EndsWith("}")) || (input.StartsWith("[") && input.EndsWith("]")))
            {
                try
                {
                    var obj = JToken.Parse(input);
                    return true;
                }
                catch (JsonReaderException jex)
                {
                    Console.WriteLine(jex.Message);
                    return false;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
    }
}