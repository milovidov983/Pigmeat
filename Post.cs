using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using Markdig;
using Markdig.Parsers;
using Markdig.Extensions.AutoLinks;

namespace WDHAN
{
    public class Post : Page
    {
        public Post()
        {
            
        }
        public static Post getDefinedPost(Post post, string collection)
        {
            var currentPost = (Post) getDefinedPage(post);
            currentPost.url = Permalink.GetPermalink(currentPost).parsePostPermalink(collection, currentPost);
            return currentPost;
        }
        public static List<Post> getPosts(string collection)
        {
            var siteConfig = GlobalConfiguration.getConfiguration();
            List<Post> postList = new List<Post>();
            var builder = new MarkdownPipelineBuilder().UseAdvancedExtensions();
            builder.BlockParsers.TryRemove<IndentedCodeBlockParser>();
            var pipeline = builder.Build();
            builder.Extensions.Remove(pipeline.Extensions.Find<AutoLinkExtension>());


            foreach(var post in Directory.GetFiles(siteConfig.collections_dir + "/_" + collection))
            {
                if(GlobalConfiguration.isMarkdown(Path.GetExtension(post).Substring(1)))
                {
                    if(!Path.GetFileNameWithoutExtension(post).Equals("index", StringComparison.OrdinalIgnoreCase))
                    {
                        postList.Add(getDefinedPost(new Post() { frontmatter = parseFrontMatter(post),
                        content = Markdown.ToHtml(WDHANFile.parseRaw(post), pipeline),
                        path = post }, collection));
                    }
                }
            }
            
            try
            {
                postList.Sort((y, x) => x.frontmatter["date"].ToString().CompareTo(y.frontmatter["date"].ToString()));
                postList.Sort((y, x) => x.title.CompareTo(y.title));
            }
            catch(NullReferenceException)
            {
                postList.Sort((y, x) => x.title.CompareTo(y.title));
            }

            return postList;
        }
        public static void generateEntries()
        {
            var siteConfig = GlobalConfiguration.getConfiguration();
            foreach(var collection in siteConfig.collections)
            {
                Collection collectionPosts = new Collection();
                collectionPosts.entries = getPosts(collection);
                string collectionSerialized = JsonConvert.SerializeObject(collectionPosts, Formatting.Indented);
                Directory.CreateDirectory(siteConfig.source + "/temp/_" + collection);
                using (FileStream fs = File.Create(siteConfig.source + "/temp/_" + collection + "/_entries.json"))
                {
                    fs.Write(Encoding.UTF8.GetBytes(collectionSerialized), 0, Encoding.UTF8.GetBytes(collectionSerialized).Length);
                }
            }
        }
    }
}