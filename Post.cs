using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace WDHAN
{
    public class Post
    {
        public JObject frontmatter { get; set; }
        public string content { get; set; }
        public Post()
        {
            
        }
        public static List<string> getPosts(string collectionName) 
        {
            var siteConfig = GlobalConfiguration.getConfiguration();
            List<string> postList = new List<string>();
            foreach(var collection in siteConfig.collections)
            {
                if(collection.Equals(collectionName, StringComparison.OrdinalIgnoreCase))
                {
                    foreach(var post in Directory.GetFiles(siteConfig.collections_dir + "/_" + collection))
                    {
                        if(GlobalConfiguration.getMarkdownExts().Contains(Path.GetExtension(post)))
                        {
                            postList.Add(File.ReadAllText(post));
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
            using (FileStream fs = File.Create("./_" + collectionName + "/entries.json"))
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
                    return JObject.Parse(pageFrontMatter);
                }
            }
            else
            {
                return JObject.Parse("{\"null\": true}");
            }
        }
    }
}