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
        public static Post getDefinedPost(Post post)
        {
            return (Post) getDefinedPage(post);

            /*
            post.name = Path.GetFileName(post.path);
            post.dir = Path.GetDirectoryName(post.path);

            try
            {
                post.tags = JsonConvert.DeserializeObject<List<string>>(post.frontmatter.GetValue("tags").ToString());
            }
            catch(NullReferenceException)
            {

            }

            try
            {
                post.date = JsonConvert.DeserializeObject<DateTime>(post.frontmatter.GetValue("date").ToString());
            }
            catch(NullReferenceException)
            {

            }

            try
            {
                post.excerpt = getExcerpt(post);
            }
            catch(NullReferenceException)
            {

            }

            return post;
            */
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
                            if(!Path.GetFileNameWithoutExtension(post).Equals("index", StringComparison.OrdinalIgnoreCase))
                            {
                                postList.Add(getDefinedPost(new Post() { frontmatter = parseFrontMatter(post),
                                content = WDHAN.Program.parsePage(collectionName, post, WDHANFile.getFileContents(post), false),
                                path = post }));
                            }
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