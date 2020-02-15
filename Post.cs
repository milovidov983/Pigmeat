using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace WDHAN
{
    public class Post : Page
    {
        public Post()
        {
            
        }
        public static List<Post> getPosts(string collectionName)
        {
            var siteConfig = GlobalConfiguration.getConfiguration();
            List<Post> postList = new List<Post>();
            foreach(var collection in siteConfig.collections)
            {
                if(collection.Equals(collectionName, StringComparison.OrdinalIgnoreCase))
                {
                    foreach(var post in Directory.GetFiles(siteConfig.collections_dir + "/_" + collection))
                    {
                        if(GlobalConfiguration.isMarkdown(Path.GetExtension(post).Substring(1)))
                        {
                            postList.Add(new Post() { frontmatter = parseFrontMatter(post), content = Page.getPageContents(post), path = post });
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
            var siteConfig = GlobalConfiguration.getConfiguration();
            Collection collectionPosts = new Collection();
            collectionPosts.entries = getPosts(collectionName);
            string collectionSerialized = JsonConvert.SerializeObject(collectionPosts, Formatting.Indented);
            Directory.CreateDirectory(siteConfig.source + "/temp/_" + collectionName);
            using (FileStream fs = File.Create(siteConfig.source + "/temp/_" + collectionName + "/_entries.json"))
            {
                fs.Write(Encoding.UTF8.GetBytes(collectionSerialized), 0, Encoding.UTF8.GetBytes(collectionSerialized).Length);
            }
        }
    }
}