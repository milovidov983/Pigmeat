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
        public static string getPosts(string collectionName) 
        {
            var siteConfig = GlobalConfiguration.getConfiguration();
            List<Post> postList = new List<Post>();
            foreach(var collection in siteConfig.collections)
            {
                if(collection.Equals(collectionName, StringComparison.OrdinalIgnoreCase))
                {
                    foreach(var post in Directory.GetFiles(siteConfig.collections_dir + "/_" + collection))
                    {
                        if(GlobalConfiguration.getMarkdownExts().Contains(Path.GetExtension(post)))
                        {
                            postList.Add(new Post { frontmatter = parseFrontMatter(post), content = File.ReadAllText(post) });
                        }
                    }
                }
            }
            Console.WriteLine(JsonConvert.SerializeObject(postList, Formatting.Indented));
            return JsonConvert.SerializeObject(postList, Formatting.Indented);
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