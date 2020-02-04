using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json.Linq;

namespace WDHAN
{
    public class Post
    {
        public string title { get; set; }
        public JObject frontmatter { get; set; }
        public string content { get; set; }
        public Post()
        {
            
        }
        public static JObject getPosts(string collectionName, Config siteConfig) 
        {
            List<string> postList = new List<string>();
            foreach(var collection in siteConfig.collections)
            {
                foreach(var key in collection.Keys)
                {
                    if(key.Equals(collectionName, StringComparison.OrdinalIgnoreCase))
                    {
                        foreach(var post in Directory.GetFiles(siteConfig.collections_dir + "/_" + key))
                        {
                            postList.Add(File.ReadAllText(post));
                        }
                    }
                }
            }
            return JObject.FromObject(postList);
        }
    }
}